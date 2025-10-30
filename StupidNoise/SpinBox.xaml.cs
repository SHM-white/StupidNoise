using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StupidNoise
{
    /// <summary>
    /// SpinBox.xaml 的交互逻辑
    /// </summary>
    [ContentProperty("Value")]
    public partial class SpinBox : UserControl
    {
        public SpinBox()
        {
            InitializeComponent();
        }
        /// <summary>
        /// DepedencyProperty for Step
        /// </summary>
        public static readonly DependencyProperty StepProperty
            = DependencyProperty.Register("Step", typeof(double),
                typeof(SpinBox), new PropertyMetadata(1.0));

        /// <summary>
        /// DepedencyProperty for Value
        /// </summary>
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register("Value", typeof(double),
                typeof(SpinBox), new FrameworkPropertyMetadata(0.0,
                     FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                    | FrameworkPropertyMetadataOptions.Journal
                    | FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnValueChanged))
                );


        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set
            {
                if (Value != value)
                {
                    SetValue(ValueProperty, value);
                }
            }
        }

        public double Step
        {
            get => (double)GetValue(StepProperty);
            set
            {
                if (Step != value)
                {
                    SetValue(StepProperty, value);
                }
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spinBox = d as SpinBox;
            if (spinBox != null)
            {
                spinBox.txtBoxValue.Text = e.NewValue.ToString();
            }
        }

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            Value += Step;
        }

        private void btnMinor_Click(object sender, RoutedEventArgs e)
        {
            Value -= Step;
        }
    }
}
