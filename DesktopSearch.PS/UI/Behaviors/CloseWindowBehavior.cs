using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace DesktopSearch.PS.UI
{
    public class CloseWindowBehavior : Behavior<Window>
    {
        public string CloseTrigger
        {
            get { return (string)GetValue(CloseTriggerProperty); }
            set { SetValue(CloseTriggerProperty, value); }
        }

        public static readonly DependencyProperty CloseTriggerProperty =
            DependencyProperty.Register("CloseTrigger", typeof(string), typeof(CloseWindowBehavior), new PropertyMetadata(null, OnCloseTriggerChanged));

        private static void OnCloseTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as CloseWindowBehavior;

            if (behavior != null)
            {
                behavior.OnCloseTriggerChanged();
            }
        }

        private void OnCloseTriggerChanged()
        {
            if (string.IsNullOrWhiteSpace(this.CloseTrigger))
                return;

            // when closetrigger is true, close the window
            if (this.CloseTrigger == "OK")
            {
                this.AssociatedObject.DialogResult = true;
            }
            else
            {
                this.AssociatedObject.DialogResult = false;
            }

            this.AssociatedObject.Close();
        }
    }
}
