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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;

enum TimeTarget
{
    Seconds,Minutes
}

namespace TimeShut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/sg /t 0")
        {
            CreateNoWindow = false,
            UseShellExecute = false,
        };

        bool secs = false;
        bool started = false;
        int seconds;
        int mins;

        static Timer timer;
        int MINUTE = 60;
        int TIMERINTERVAL = 1000;

        public MainWindow()
        {
            InitializeComponent();
            seconds = Int32.Parse(Sec.Text);
            updateSwitchBtns();
            
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void LinkHandler(object sender, MouseButtonEventArgs e)
        {
            string data = (sender as Image).Name;
            string url = "";

            switch (data)
            {
                case "github":
                    url = "https://github.com/ato-codes";
                    break;
                case "insta":
                    url = "https://www.instagram.com/ato.codes/";
                    break;
                case "tiktok":
                    url = "https://www.tiktok.com/@atocodes?_t=8irnKvPcltt&_r=1";
                    break;
                case "telegram":
                    url = "https://t.me/atocodes/";
                    break;
            }

            Process.Start(new ProcessStartInfo(url));
            e.Handled = true;
        }

        private void UpdateTimeOnScroll(object sender, MouseWheelEventArgs e)
        {
            if (seconds >= 59)
            {
                mins += 1;
                seconds = 0;
            }else if (secs)
            {
                _ = e.Delta > 0 ? seconds += 1 : seconds > 0 ? seconds -= 1 : 0;
            }
            else
            {
                _ = e.Delta > 0 ? mins += 1 : mins > 0 ? mins -= 1 : 0;
            }

            Sec.Text = seconds.ToString();
            Mins.Text = mins.ToString();

            UpdateStartBtn();
        }


        private void UpdateStartBtn()
        {
            _ = seconds > 0 || mins > 0 ? StartBtn.IsEnabled = true : StartBtn.IsEnabled = false;

        }

        private void updateSwitchBtns()
        {
            secs = !secs;
            MinBtn.Style = secs ? (Style)TryFindResource("normal-btn") : (Style)TryFindResource("active-btn");
            SecBtn.Style = !secs ? (Style)TryFindResource("normal-btn") : (Style)TryFindResource("active-btn");
        }

        private void updateUI()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (seconds == 0) StartBtn.IsEnabled = false;
                Options.Visibility = started ? Visibility.Hidden : Visibility.Visible;
                StartBtn.Visibility = started ? Visibility.Hidden : Visibility.Visible;
                StopBtn.Visibility = started ? Visibility.Visible : Visibility.Hidden;
            });
        }

        private void SecBtn_Click(object sender, RoutedEventArgs e)
        {
            updateSwitchBtns();
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            updateSwitchBtns();
        }

        private void StartTimer(object sender, RoutedEventArgs e)
        {
            started = true;
            timer = new Timer(TimerCallback, null, seconds, TIMERINTERVAL);
            updateUI();
        }

        private void StopTimer(object sender, RoutedEventArgs e)
        {
            started = false;
            timer.Dispose();
            updateUI();
            return;
        }

        private void TimerCallback(object state)
        {

            if (!started || seconds == 0 && mins == 0)
            {
                StopTimer(null,null);
                Process.Start(psi);
                return;
            }

            if (mins > 0 && seconds == 0)
            {
                mins -= 1;
                seconds = MINUTE;
            }

            seconds -= 1;

            this.Dispatcher.Invoke(() =>
            {
                Sec.Text = seconds.ToString();
                Mins.Text = mins.ToString();
            });

            updateUI();

        }

        private void UpdateValue(bool inc = true)
        {
            if(seconds >= 59)
            {
                seconds = 0;
                mins += 1;

            }else if (secs) {
                _ = inc ? seconds += 1 : seconds > 0 ? seconds -= 1 : seconds ;
            }
            else
            {
                _ = inc ? mins += 1 : mins > 0 ? mins -= 1 : mins;
            }

            Sec.Text = seconds.ToString();
            Mins.Text = mins.ToString();

            UpdateStartBtn();
        }

        private void Reset()
        {
            mins = 0;
            seconds = 0;
            Mins.Text = mins.ToString();
            Sec.Text = seconds.ToString();
            UpdateStartBtn();
        }

        private void AppKeyMaps(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    UpdateValue(inc: true);
                    break;
                case Key.Down:
                    UpdateValue(inc : false);
                    break;
                case Key.LeftShift:
                    updateSwitchBtns();
                    break;
                case Key.Space:
                    if(!started)
                        StartTimer(null, null);
                    else
                        StopTimer(null, null);
                    break;
                case Key.Escape:
                    Reset();
                    break;
            }

        }

        private void Quit(object sender, RoutedEventArgs e) =>
            App.Current.Shutdown();
    }
}
