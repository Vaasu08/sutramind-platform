using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SutraMind.Desktop.Resources;

public static class ClinicalColorAnimation
{
    public static readonly DependencyProperty AnimateBackgroundProperty =
        DependencyProperty.RegisterAttached(
            "AnimateBackground",
            typeof(bool),
            typeof(ClinicalColorAnimation),
            new PropertyMetadata(false, OnAnimateBackgroundChanged));

    public static void SetAnimateBackground(DependencyObject element, bool value) =>
        element.SetValue(AnimateBackgroundProperty, value);

    public static bool GetAnimateBackground(DependencyObject element) =>
        (bool)element.GetValue(AnimateBackgroundProperty);

    private static void OnAnimateBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not System.Windows.Controls.Button button)
            return;

        button.MouseEnter += (_, _) => Animate(button, Colors.Transparent, Color.FromRgb(0x1B, 0x51, 0x4A));
        button.MouseLeave += (_, _) => Animate(button, Color.FromRgb(0x1B, 0x51, 0x4A), Colors.Transparent);
    }

    private static void Animate(System.Windows.Controls.Button button, Color from, Color to)
    {
        if (button.Background is not SolidColorBrush brush)
            brush = new SolidColorBrush(from);

        button.Background = brush;
        brush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(from, to, TimeSpan.FromMilliseconds(200)));
    }
}
