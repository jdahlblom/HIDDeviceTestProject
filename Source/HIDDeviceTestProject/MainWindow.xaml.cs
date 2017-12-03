using System;
using System.Windows;
using System.Windows.Controls;
using HidLibrary;

namespace HIDDeviceTestProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int _vendorId = 0;
        private int _productId = 0;
        private string _instanceId = "";
        private HidDevice _hidDevice = null;


        public MainWindow()
        {
            InitializeComponent();
        }


        private void OnReport(HidReport report)
        {
            try
            {
                try
                {
                    var result = "";
                    foreach (var b in report.Data)
                    {
                        result = result + b.ToString() + " ";
                    }
                    AddHidData(result);
                }
                catch (Exception ex)
                {
                    AddException(ex);
                }
                try
                {
                    if (_hidDevice != null && _hidDevice.IsOpen)
                    {
                        _hidDevice.ReadReport(OnReport);
                    }
                }
                catch (Exception ex)
                {
                    AddException(ex);
                }
            }
            catch (Exception ex)
            {
                AddException(ex);
            }
        }


        private void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _vendorId = Convert.ToInt32(TextBoxVendorId.Text, 16);
                _productId = Convert.ToInt32(TextBoxProductId.Text, 16);
                foreach (var hidDevice in HidDevices.Enumerate(_vendorId, _productId))
                {
                    if (hidDevice != null)
                    {
                        _hidDevice = hidDevice;
                        _hidDevice.OpenDevice(DeviceMode.NonOverlapped, DeviceMode.NonOverlapped, ShareMode.ShareRead | ShareMode.ShareWrite);
                        _hidDevice.ReadReport(OnReport);
                        AddDebugData("Device opened.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                AddException(ex);
            }
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _vendorId = -1;
                _productId = -1;

                if (_hidDevice != null && _hidDevice.IsOpen)
                {
                    _hidDevice.CloseDevice();
                    _hidDevice = null;
                    AddDebugData("Device closed.");
                }
            }
            catch (Exception ex)
            {
                AddException(ex);
            }
        }

        private void TextBoxVendorId_OnTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBoxProductId_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void AddHidData(string str)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBoxRawData.Text = str + Environment.NewLine + TextBoxRawData.Text;
            });
        }

        private void AddDebugData(int i)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBoxExceptions.Text = i + Environment.NewLine + TextBoxExceptions.Text;
            });
        }

        private void AddDebugData(string str)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBoxExceptions.Text = str + Environment.NewLine + TextBoxExceptions.Text;
            });
        }

        private void AddException(Exception ex)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBoxExceptions.Text = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + TextBoxExceptions.Text;
            });
        }
    }
}
