using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MVVMLight.Messaging;
using Zenfolio7.Messages;
using Zenfolio7.View.ViewModel;
using Zenfolio7.Zenfolio;

namespace Zenfolio7.ZenSync
{
    public class ZenSyncViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public GroupElement SelectedGroupElement { get; set; }
        public string SelectedDirectory { get; set; }
        public string SyncProcessUpdate { get; set; }
        public RelayCommand SyncCommand { get; set; }
        public string SyncButtonText { get; set; }
        public bool IsSyncing { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public string CancelButtonText { get; set; }
        private CancellationTokenSource _cts;
        private static Thread _uploadThread;
        public ZenSyncViewModel() 
        {
            Messenger.Default.Register<DataUpdateMessage>(this, UpdateData);
            SyncCommand = new RelayCommand(Sync);
            CancelCommand = new RelayCommand(Cancel);

            SyncButtonText = "Sync!";
            CancelButtonText = "Cancel";
        }

        public bool CanSync
        {
            get
            {
                return Directory.Exists(SelectedDirectory) && SelectedGroupElement != null && !IsSyncing;
            }
        }
        public bool CanCancel { get { return IsSyncing; } }

        private async void Sync(object obj)
        {
            IsSyncing = true;
            SyncProcessUpdate = string.Empty;
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            _cts = new CancellationTokenSource();
            //_cts.Token.Register(() =>
            //{
            //    tcs.TrySetCanceled();    
            //});

            //using (var _cts = new CancellationTokenSource())
            {
                await DirectoryIteratoryAsync(_cts);
            }
            
            //var completedTask = await Task.WhenAny(task, tcs.Task);

            //try
            //{
            //    await DirectoryIteratoryAsync();
            //}
            //catch (TaskCanceledException)
            //{

            //}

        }

        private void Cancel(object obj)
        {
            // Cancel the task
            if (_cts != null)
            {
                _cts.Cancel();
            }
            if (_uploadThread != null)
            {
                _uploadThread.Abort();
            }
        }
        private Task DirectoryIteratoryAsync(CancellationTokenSource cts)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    //using (cts.Token.Register(Thread.CurrentThread.Abort))
                    {
                        var dirContents = Directory.GetDirectories(SelectedDirectory, "*.*", SearchOption.AllDirectories).ToList();
                        dirContents.AddRange(Directory.GetFiles(SelectedDirectory, "*.*", SearchOption.AllDirectories).ToList());
                        foreach (var dir in dirContents)
                        {
                            if (cts.IsCancellationRequested)
                            {
                                throw new TaskCanceledException($"Task Cancelled on dir {dir}");
                            }
                            _uploadThread = SyncFileThread(dir, 300);
                            _uploadThread.Start();
                            SyncProcessUpdate += dir + "\n";
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    //do something?
                }
                catch (ThreadAbortException)
                {
                    //do something else
                }
                catch (Exception ex)
                {
                    //do something else
                }
                finally
                {
                    IsSyncing = false;
                }
            });
        }

        private Thread SyncFileThread(string dir, int time)
        {
            
            return new Thread(() =>
            {
                try
                {
                    Task.Delay(time).Wait();
                }
                catch (Exception ex)
                {
                    //do something
                    throw ex;
                }
            });
        }

        private void UpdateData(DataUpdateMessage obj)
        {
            if (obj.Packet.GetType() == typeof(string))
            {
                SelectedDirectory = (string)(obj.Packet);
            }
            if (obj.Packet.GetType() == typeof(Group))
            {
                SelectedGroupElement = (Group)(obj.Packet);
            }
            if (obj.Packet.GetType() == typeof(PhotoSet))
            {
                SelectedGroupElement = (PhotoSet)(obj.Packet);
            }
        }
    }
}