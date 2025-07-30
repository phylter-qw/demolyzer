using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace QuantumBitDesigns.Charting
{
    // MODIFIED FROM http://marlongrech.wordpress.com/avalon-controls-library/

    /// <summary>
    /// A slider that provides the a range
    /// </summary>
    [DefaultEvent("RangeSelectionChanged"),
    TemplatePart(Name = "PART_RangeSliderContainer", Type = typeof(StackPanel)),
    TemplatePart(Name = "PART_LeftEdge", Type = typeof(RepeatButton)),
    TemplatePart(Name = "PART_RightEdge", Type = typeof(RepeatButton)),
    TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb)),
    TemplatePart(Name = "PART_MiddleThumb", Type = typeof(Thumb)),
    TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
    public sealed class ChartRangeSlider : Control
    {
        #region Data members
        bool internalUpdate = false;
        const double RepeatButtonMoveRatio = 0.1;//used to move the selection by x ratio when click the repeat buttons
        const double DefaultSplittersThumbWidth = 5;
        Thumb centerThumb; //the center thumb to move the range around
        Thumb leftThumb;//the left thumb that is used to expand the range selected
        Thumb rightThumb;//the right thumb that is used to expand the range selected
        Rectangle leftButton;//the left side of the control (movable left part)
        Rectangle rightButton;//the right side of the control (movable right part)
        StackPanel visualElementsContainer;//stackpanel to store the visual elements for this control
        #endregion

        #region properties and events

        /// <summary>
        /// The max value for the range of the range slider
        /// </summary>
        public long Maximum
        {
            get { return (long)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// The min value of the selected range of the range slider
        /// </summary>
        public long ValueStart
        {
            get { return (long)GetValue(ValueStartProperty); }
            set { SetValue(ValueStartProperty, value); }
        }

        /// <summary>
        /// The max value of the selected range of the range slider
        /// </summary>
        public long ValueEnd
        {
            get { return (long)GetValue(ValueEndProperty); }
            set { SetValue(ValueEndProperty, value); }
        }

        /// <summary>
        /// The max value for the range of the range slider
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(long), typeof(ChartRangeSlider), new UIPropertyMetadata((long)1, new PropertyChangedCallback(RangeChanged)));

        /// <summary>
        /// The min value of the selected range of the range slider
        /// </summary>
        public static readonly DependencyProperty ValueStartProperty = DependencyProperty.Register("ValueStart", typeof(long), typeof(ChartRangeSlider), new UIPropertyMetadata((long)0, new PropertyChangedCallback(SelectionChanged)));

        /// <summary>
        /// The max value of the selected range of the range slider
        /// </summary>
        public static readonly DependencyProperty ValueEndProperty = DependencyProperty.Register("ValueEnd", typeof(long), typeof(ChartRangeSlider), new UIPropertyMetadata((long)1, new PropertyChangedCallback(SelectionChanged)));

        private static void RangeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ChartRangeSlider slider = (ChartRangeSlider)sender;
            if (!slider.internalUpdate)//check if the property is set internally
            {
                slider.ReCalculateRanges();
                slider.ReCalculateWidths();
            }
        }

        private static void SelectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ChartRangeSlider slider = (ChartRangeSlider)sender;
            if (!slider.internalUpdate)//check if the property is set internally
            {
                slider.ReCalculateWidths();
                slider.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(slider));
            }
        }

        /// <summary>
        /// Event raised whenever the selected range is changed
        /// </summary>
        public static readonly RoutedEvent RangeSelectionChangedEvent =
            EventManager.RegisterRoutedEvent("RangeSelectionChanged",
            RoutingStrategy.Bubble, typeof(RangeSelectionChangedEventHandler), typeof(ChartRangeSlider));

        /// <summary>
        /// Event raised whenever the selected range is changed
        /// </summary>
        public event RangeSelectionChangedEventHandler RangeSelectionChanged
        {
            add { AddHandler(RangeSelectionChangedEvent, value); }
            remove { RemoveHandler(RangeSelectionChangedEvent, value); }
        }


        #endregion

        #region Commands

        /// <summary>
        /// Command to move back the selection
        /// </summary>
        public static RoutedUICommand MoveBack = new RoutedUICommand("MoveBack", "MoveBack", typeof(ChartRangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Control)}));

        /// <summary>
        /// Command to move forward the selection
        /// </summary>
        public static RoutedUICommand MoveForward = new RoutedUICommand("MoveForward", "MoveForward", typeof(ChartRangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Control)}));

        /// <summary>
        /// Command to move all forward the selection
        /// </summary>
        public static RoutedUICommand MoveAllForward = new RoutedUICommand("MoveAllForward", "MoveAllForward", typeof(ChartRangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Alt)}));

        /// <summary>
        /// Command to move all back the selection
        /// </summary>
        public static RoutedUICommand MoveAllBack = new RoutedUICommand("MoveAllBack", "MoveAllBack", typeof(ChartRangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Alt) }));

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public ChartRangeSlider()
        {
            CommandBindings.Add(new CommandBinding(MoveBack, MoveBackHandler));
            CommandBindings.Add(new CommandBinding(MoveForward, MoveForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllForward, MoveAllForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllBack, MoveAllBackHandler));

            //hook to the size change event of the range slider
            DependencyPropertyDescriptor.FromProperty(ActualWidthProperty, typeof(ChartRangeSlider)). AddValueChanged(this, delegate { ReCalculateWidths(); });
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static ChartRangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartRangeSlider), new FrameworkPropertyMetadata(typeof(ChartRangeSlider)));
        }

        #region Command handlers

        void MoveAllBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ResetSelection(true);
        }

        void MoveAllForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ResetSelection(false);
        }

        void MoveBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            MoveSelection(true);
        }

        void MoveForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            MoveSelection(false);
        }
        #endregion

        #region event handlers for visual elements to drag the range
        //drag thumb from the right splitter
        private void RightThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveThumb(centerThumb, rightButton, e.HorizontalChange);
            ReCalculateRangeSelected(false, true);
        }

        //drag thumb from the left splitter
        private void LeftThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveThumb(leftButton, centerThumb, e.HorizontalChange);
            ReCalculateRangeSelected(true, false);
        }

        //left repeat button clicked
        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            MoveSelection(true);
        }
        //right repeat button clicked
        private void RightButtonClick(object sender, RoutedEventArgs e)
        {
            MoveSelection(false);
        }

        //drag thumb from the middle
        private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveThumb(leftButton, rightButton, e.HorizontalChange);
            ReCalculateRangeSelected(true, true);
        }
        #endregion

        #region logic to resize range
        //resizes the left column and the right column
        private static void MoveThumb(FrameworkElement x, FrameworkElement y, double horizonalChange)
        {
            double change = 0;
            if (horizonalChange < 0) //slider went left
                change = GetChangeKeepPositive(x.Width, horizonalChange);
            else if (horizonalChange > 0) //slider went right if(horizontal change == 0 do nothing)
                change = -GetChangeKeepPositive(y.Width, -horizonalChange);

            x.Width += change;
            y.Width -= change;
        }

        //ensures that the new value (newValue param) is a valid value. returns false if not
        private static double GetChangeKeepPositive(double width, double increment)
        {
            return Math.Max(width + increment, 0) - width;
        }
        #endregion

        #region logic to calculate the range
        long movableRange = 0;
        double movableWidth = 0;

        //recalculates the movableRange. called from the RangeStop setter, RangeStart setter and MinRange setter
        private void ReCalculateRanges()
        {
            movableRange = Maximum;
        }

        //recalculates the movableWidth. called whenever the width of the control changes
        private void ReCalculateWidths()
        {
            if (leftButton != null && rightButton != null && centerThumb != null)
            {
                movableWidth = Math.Max(ActualWidth - rightThumb.ActualWidth - leftThumb.ActualWidth - centerThumb.MinWidth, 1);
                leftButton.Width = Math.Max(movableWidth * ValueStart / movableRange, 0);
                rightButton.Width = Math.Max(movableWidth * (Maximum - ValueEnd) / movableRange, 0);
                centerThumb.Width = Math.Max(ActualWidth - leftButton.Width - rightButton.Width - rightThumb.ActualWidth - leftThumb.ActualWidth, 0);
            }
        }

        //recalculates the rangeStartSelected called when the left thumb is moved and when the middle thumb is moved
        //recalculates the rangeStopSelected called when the right thumb is moved and when the middle thumb is moved
        private void ReCalculateRangeSelected(bool reCalculateStart, bool reCalculateStop)
        {
            internalUpdate = true;//set flag to signal that the properties are being set by the object itself
            if (reCalculateStart)
            {
                // Make sure to get exactly rangestart if thumb is at the start
                if (leftButton.Width == 0.0)
                    ValueStart = 0;
                else
                    ValueStart = (long)(movableRange * leftButton.Width / movableWidth);
            }

            if (reCalculateStop)
            {
                // Make sure to get exactly rangestop if thumb is at the end
                if (rightButton.Width == 0.0)
                    ValueEnd = Maximum;
                else
                    ValueEnd =
                        Math.Min(Maximum, (long)(Maximum - movableRange * rightButton.Width / movableWidth));
            }

            internalUpdate = false;//set flag to signal that the properties are being set by the object itself

            if (reCalculateStart || reCalculateStop)
                //raise the RangeSelectionChanged event
                OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        /// <summary>
        /// moves the current selection with x value
        /// </summary>
        /// <param name="isLeft">True if you want to move to the left</param>
        public void MoveSelection(bool isLeft)
        {
            double widthChange = RepeatButtonMoveRatio * (ValueEnd - ValueStart)
                * movableWidth / movableRange;

            widthChange = isLeft ? -widthChange : widthChange;
            MoveThumb(leftButton, rightButton, widthChange);
            ReCalculateRangeSelected(true, true);
        }

        /// <summary>
        /// Reset the Slider to the Start/End
        /// </summary>
        /// <param name="isStart">Pass true to reset to start point</param>
        public void ResetSelection(bool isStart)
        {
            double widthChange = Maximum;
            widthChange = isStart ? -widthChange : widthChange;

            MoveThumb(leftButton, rightButton, widthChange);
            ReCalculateRangeSelected(true, true);
        }

        #endregion

        //Raises the RangeSelectionChanged event
        private void OnRangeSelectionChanged(RangeSelectionChangedEventArgs e)
        {
            e.RoutedEvent = RangeSelectionChangedEvent;
            RaiseEvent(e);
        }

        /// <summary>
        /// Overide to get the visuals from the control template
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            visualElementsContainer = EnforceInstance<StackPanel>("PART_RangeSliderContainer");
            centerThumb = EnforceInstance<Thumb>("PART_MiddleThumb");
            leftButton = EnforceInstance<Rectangle>("PART_LeftEdge");
            rightButton = EnforceInstance<Rectangle>("PART_RightEdge");
            leftThumb = EnforceInstance<Thumb>("PART_LeftThumb");
            rightThumb = EnforceInstance<Thumb>("PART_RightThumb");
            InitializeVisualElementsContainer();
            ReCalculateWidths();
        }

        #region Helper
        T EnforceInstance<T>(string partName)
            where T : FrameworkElement, new()
        {
            T element = GetTemplateChild(partName) as T;
            if (element == null)
                element = new T();
            return element;
        }

        //adds all visual element to the conatiner
        private void InitializeVisualElementsContainer()
        {
            visualElementsContainer.Orientation = Orientation.Horizontal;
            leftThumb.Width = DefaultSplittersThumbWidth;
            leftThumb.Tag = "left";
            rightThumb.Width = DefaultSplittersThumbWidth;
            rightThumb.Tag = "right";

            //handle the drag delta
            centerThumb.DragDelta += CenterThumbDragDelta;
            leftThumb.DragDelta += LeftThumbDragDelta;
            rightThumb.DragDelta += RightThumbDragDelta;
        }
        #endregion
    }

    /// <summary>
    /// Delegate for the RangeSelectionChanged event
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void RangeSelectionChangedEventHandler(object sender, RangeSelectionChangedEventArgs e);

    /// <summary>
    /// Event arguments for the Range slider RangeSelectionChanged event
    /// </summary>
    public class RangeSelectionChangedEventArgs : RoutedEventArgs
    {
        private long newRangeStart;

        /// <summary>
        /// The new range start selected in the range slider
        /// </summary>
        public long NewRangeStart
        {
            get { return newRangeStart; }
            set { newRangeStart = value; }
        }

        private long newRangeStop;

        /// <summary>
        /// The new range stop selected in the range slider
        /// </summary>
        public long NewRangeStop
        {
            get { return newRangeStop; }
            set { newRangeStop = value; }
        }

        /// <summary>
        /// sets the range start and range stop for the event args
        /// </summary>
        /// <param name="newRangeStart">The new range start set</param>
        /// <param name="newRangeStop">The new range stop set</param>
        internal RangeSelectionChangedEventArgs(long newRangeStart, long newRangeStop)
        {
            this.newRangeStart = newRangeStart;
            this.newRangeStop = newRangeStop;
        }

        /// <summary>
        /// sets the range start and range stop for the event args by using the slider RangeStartSelected and RangeStopSelected properties
        /// </summary>
        /// <param name="slider">The slider to get the info from</param>
        internal RangeSelectionChangedEventArgs(ChartRangeSlider slider)
            : this(slider.ValueStart, slider.ValueEnd)
        { }

    }
}
