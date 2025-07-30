using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Demolyzer.Model
{
    public static class StopwatchUtil
    {
        private static double frequency = (double)Stopwatch.Frequency;

        public static double ElapsedDuration(this Stopwatch sw)
        {
            return (double)sw.ElapsedTicks / frequency;
        }
    }

    public class MatchRunner
    {
        public event EventHandler<PackEventArgs> PackDropped;
        public event EventHandler<PackEventArgs> PackPickup;

        private DemoContent _demoContent;
        private MatchProcessor _processor;
        private int _currentPacketIndex;
        private int _packetCount;
        private bool _isExecuting;

        public MatchRunner(DemoContent demoContent)
        {
            this._processor = new MatchProcessor(demoContent.Entities.ToArray());
            this.Rate = 1;

            //let the processor know which players are spectators and players
            for (int i = 0; i < demoContent.FinalPlayerStats.Length; ++i)
            {
                //this._processor.Players[i].IsSpectator = demoContent.FinalPlayerStats[i].IsSpectator;
                this._processor.Players[i].CurrentStats.IsSpectator = demoContent.FinalPlayerStats[i].IsSpectator;
                this._processor.Players[i].CurrentStats.TeamIndex = demoContent.FinalPlayerStats[i].TeamIndex;
                this._processor.Players[i].CurrentStats.Team = demoContent.FinalPlayerStats[i].Team;
                this._processor.Players[i].CurrentStats.Name = demoContent.FinalPlayerStats[i].Name;
            }

            this._processor.PackDropped += processor_PackDropped;
            this._processor.PackPickup += processor_PackPickup;
            this._demoContent = demoContent;
            this._packetCount = demoContent.Packets.Count;
        }

        void processor_PackPickup(object sender, PackEventArgs e)
        {
            OnPackPickup(e);
        }

        void processor_PackDropped(object sender, PackEventArgs e)
        {
            OnPackDropped(e);
        }

        public void Play()
        {
            if (this._runnerStopwatch != null && this._runnerStopwatch.IsRunning == false)
            {
                this._runnerStopwatch.Start();
                return;
            }
            //if (this._isPaused == true)
            //{
            //    this._isPaused = false;
            //    return;
            //}

            if (this._isExecuting == true)
            {
                return;
            }
            this._isExecuting = true;
            Task.Factory.StartNew(ExecutePackets, TaskCreationOptions.LongRunning);
        }

        private Stopwatch _runnerStopwatch;

        public double Rate { get; set; }

        public bool IsExecuting
        {
            get
            {
                return this._isExecuting;
            }
        }

        private void ExecutePackets()
        {
            if (this._isSeekToIndexEnabled == false)
            {
                int matchStartedIndex = 0;
                for (int i = 0; i < this._demoContent.Packets.Count; ++i)
                {
                    if (this._demoContent.Packets[i].Type == DemoDeltaType.MatchStarted)
                    {
                        matchStartedIndex = i;
                        break;
                    }
                }
                SeekToIndex(matchStartedIndex);
            }

            //Stopwatch sw = Stopwatch.StartNew();
            this._runnerStopwatch = Stopwatch.StartNew();
            double lastTime = 0d;

            //Stopwatch resetStopWatch = Stopwatch.StartNew();
            double processedPacketsTime = 0d;
            double currentClock = 0d;

            while (this._currentPacketIndex < this._packetCount && this._isStopped == false)
            {
                if (this._isSeekToIndexEnabled == true)
                {
                    if (this._currentPacketIndex < this._indexToSeekTo)
                    {
                        DemoDelta nextPacket = this._demoContent.Packets[this._currentPacketIndex];
                        this._processor.Process(nextPacket);
                        this._currentPacketIndex++;

                        if (this._currentPacketIndex >= this._indexToSeekTo)
                        {
                            this._isSeekToIndexEnabled = false;

                            //reset tracking of time and execute normally now
                            //sw = Stopwatch.StartNew();
                            processedPacketsTime = 0;
                            currentClock = 0;
                            lastTime = 0;
                            this._runnerStopwatch.Restart();
                        }
                    }
                }
                else //otherwise process in normal time
                {
                    double currTime = this._runnerStopwatch.ElapsedDuration();
                    double elapsed = currTime - lastTime;
                    lastTime = currTime;
                    currentClock += this.Rate * elapsed;
                    DemoDelta nextPacket = this._demoContent.Packets[this._currentPacketIndex];
                    if (currentClock > nextPacket.DemoTime + processedPacketsTime)
                    {
                        this._processor.Process(nextPacket);
                        this._currentPacketIndex++;
                        processedPacketsTime += nextPacket.DemoTime;
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }

                    //double elapsed = sw.ElapsedDuration() * 1d;
                    //DemoDelta nextPacket = this._demoContent.Packets[this._currentPacketIndex];
                    //if (elapsed > nextPacket.DemoTime + processedPacketsTime)
                    //{
                    //    this._processor.Process(nextPacket);
                    //    this._currentPacketIndex++;
                    //    processedPacketsTime += nextPacket.DemoTime;
                    //}
                    //else
                    //{
                    //    Thread.Sleep(10);
                    //}
                }
                //while (this._isPaused == true && this._isStopped == false)
                //{
                //    if (sw.IsRunning == true)
                //    {
                //        sw.Stop();
                //    }
                //    Thread.Sleep(30);
                //}

                ////resume stopwatch if it was paused
                //if (sw.IsRunning == false)
                //{
                //    sw.Start();
                //}
            }
            this._isExecuting = false;
        }

        private bool _isStopped;

        public void Pause()
        {
            if (this._runnerStopwatch != null)
            {
                this._runnerStopwatch.Stop();
            }
            //this._isPaused = true;
        }

        public bool IsPaused
        {
            get
            {
                return this._runnerStopwatch != null && this._runnerStopwatch.IsRunning == false;
            }
        }

        public void Stop()
        {
            this._isStopped = true;
        }

        public MatchProcessor Processor
        {
            get
            {
                return this._processor;
            }
        }

        public int CurrentPacketIndex
        {
            get
            {
                if (this._isSeekToIndexEnabled == true)
                {
                    return this._indexToSeekTo;
                }
                return this._currentPacketIndex;
            }
        }

        public int PacketCount
        {
            get
            {
                return this._packetCount;
            }
        }

        private int _indexToSeekTo;
        private bool _isSeekToIndexEnabled;

        internal void SeekToIndex(int value)
        {
            if (value > this._currentPacketIndex)
            {
                this._indexToSeekTo = value;
                this._isSeekToIndexEnabled = true;
            }
        }

        protected virtual void OnPackDropped(PackEventArgs e)
        {
            if (this.PackDropped != null)
            {
                this.PackDropped(this, e);
            }
        }

        protected virtual void OnPackPickup(PackEventArgs e)
        {
            if (this.PackPickup != null)
            {
                this.PackPickup(this, e);
            }
        }
    }
}
