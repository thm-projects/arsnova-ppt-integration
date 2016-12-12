using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace ARSnovaPPIntegration.Presentation.Resources
{
    public class BindingProxy : FrameworkElement
    {
        public static readonly DependencyProperty InProperty;
        public static readonly DependencyProperty OutProperty;

        public BindingProxy()
        {
            this.Visibility = Visibility.Collapsed;
        }

        static BindingProxy()
        {
            var inMetadata = new FrameworkPropertyMetadata(
                delegate (DependencyObject p, DependencyPropertyChangedEventArgs args)
                {
                    if (null != BindingOperations.GetBinding(p, OutProperty))
                        (p as BindingProxy).Out = args.NewValue;
                });

            inMetadata.BindsTwoWayByDefault = false;
            inMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            InProperty = DependencyProperty.Register("In",
                typeof(object),
                typeof(BindingProxy),
                inMetadata);

            var outMetadata = new FrameworkPropertyMetadata(
                delegate (DependencyObject p, DependencyPropertyChangedEventArgs args)
                {
                    ValueSource source = DependencyPropertyHelper.GetValueSource(p, args.Property);

                    if (source.BaseValueSource != BaseValueSource.Local)
                    {
                        var proxy = p as BindingProxy;
                        object expected = proxy.In;
                        if (!object.ReferenceEquals(args.NewValue, expected))
                        {
                            Dispatcher.CurrentDispatcher.BeginInvoke(
                                DispatcherPriority.DataBind, new Action(() => 
                                {
                                    proxy.Out = proxy.In;
                                }));
                        }
                    }
                });

            outMetadata.BindsTwoWayByDefault = true;
            outMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            OutProperty = DependencyProperty.Register("Out",
                typeof(object),
                typeof(BindingProxy),
                outMetadata);
        }

        public object In
        {
            get { return this.GetValue(InProperty); }
            set { this.SetValue(InProperty, value); }
        }

        public object Out
        {
            get { return this.GetValue(OutProperty); }
            set { this.SetValue(OutProperty, value); }
        }
    }
}
