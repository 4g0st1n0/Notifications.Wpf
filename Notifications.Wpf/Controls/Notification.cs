using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Notifications.Wpf.Utils;

namespace Notifications.Wpf.Controls
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class Notification : ContentControl
    {
        public bool IsClosing { get; set; }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Notification));

        public static readonly RoutedEvent NotificationClosedEvent = EventManager.RegisterRoutedEvent(
            "NotificationClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Notification));

        static Notification()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Notification),
                new FrameworkPropertyMetadata(typeof(Notification)));
        }

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public event RoutedEventHandler NotificationClosed
        {
            add { AddHandler(NotificationClosedEvent, value); }
            remove { RemoveHandler(NotificationClosedEvent, value); }
        }

        public static bool GetCloseOnClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(CloseOnClickProperty);
        }

        public static void SetCloseOnClick(DependencyObject obj, bool value)
        {
            obj.SetValue(CloseOnClickProperty, value);
        }

        public static readonly DependencyProperty CloseOnClickProperty =
            DependencyProperty.RegisterAttached("CloseOnClick", typeof(bool), typeof(Notification), new FrameworkPropertyMetadata(false,CloseOnClickChanged));

        private static void CloseOnClickChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var button = dependencyObject as Button;
            if (button == null)
            {
                return;
            }

            var value = (bool)dependencyPropertyChangedEventArgs.NewValue;

            if (value)
            {
                button.Click += (sender, args) =>
                {
                    var notification = VisualTreeHelperExtensions.GetParent<Notification>(button);
                    notification?.Close();
                };
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            if (GetTemplateChild("PART_CloseButton") is Button closeButton)
            {
                closeButton.Click += OnCloseButtonOnClick;
            }

            if (Template.Resources["LoadingAnimation"] is Storyboard loadingAnimation)
            {
                if (Equals(LayoutTransform, Transform.Identity))
                {
                    LayoutTransform = new ScaleTransform(1, 1);
                }
                try
                {
                    loadingAnimation.Begin(this, Template);

                }
                catch (System.Exception)
                {
                    //throw;
                }
            }
        }

        private void OnCloseButtonOnClick(object sender, RoutedEventArgs args)
        {
            var button = sender as Button;
            if (button == null) return;
            
            button.Click -= OnCloseButtonOnClick;
            Close();
        }

        public void Close()
        {
            if (!IsClosing)
            {
                IsClosing = true;

                var closingAnimation = (Template.Resources["ClosingAnimation"] as Storyboard)?.Clone();

                if (closingAnimation == null)
                {
                    RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));

                }
                else
                {


                    try
                    {
                        closingAnimation.Completed += (sender, args) =>
                        {
                            RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));
                        };

                        closingAnimation.Begin(this, Template, true);

                    }
                    catch (System.Exception)
                    {

                        RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));

                    }
                }
            }
        }
    }
        
}
