using SlepoffStore.Core;
using SlepoffStore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlepoffStore.Tools
{
    public sealed class AlarmManager : IDisposable
    {
        private const int MAIN_TIMER_STILL_INTERVAL = 10000;
        private const int MAIN_TIMER_ALARM_INTERVAL = 1000;

        private readonly SheetsManager _sheetsManager;
        private readonly System.Threading.Timer _mainTimer;
        private SoundPlayer _player;

        public bool AlarmActivated => _player != null;

        public AlarmManager(SheetsManager sheetsManager)
        {
            _sheetsManager = sheetsManager;
            _mainTimer = new System.Threading.Timer(MainTimerCallback, null, MAIN_TIMER_STILL_INTERVAL, MAIN_TIMER_STILL_INTERVAL);
        }

        public void Dispose()
        {
            _mainTimer.Dispose();
            StopAlarm();
        }

        private void StopAlarm()
        {
            _mainTimer.Change(MAIN_TIMER_STILL_INTERVAL, MAIN_TIMER_STILL_INTERVAL);
            _player?.Stop();
            _player?.Dispose();
            _player = null;
        }

        private void StartAlarm()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Settings.AlarmRingtone))
                {
                    _player = new SoundPlayer(Settings.AlarmRingtone);
                }
                else
                {
                    _player = new SoundPlayer(Properties.Resources.Sound_19655);
                    _player.Load();
                }
                _player.PlayLooping();
                _mainTimer.Change(MAIN_TIMER_ALARM_INTERVAL, MAIN_TIMER_ALARM_INTERVAL);
            }
            catch (RemoteException ex)
            {
                ExceptionForm.ShowConnectingError(ex);
            }
        }

        private void MainTimerCallback(object? state)
        {
            if (Monitor.TryEnter(_mainTimer))
            {
                try
                {
                    var alarm = _sheetsManager.IsAlarm;

                    if (AlarmActivated)
                    {
                        if (!alarm) StopAlarm();
                    }
                    else
                    {
                        if (alarm) StartAlarm();
                    }
                }
                finally
                {
                    Monitor.Exit(_mainTimer);
                }
            }
        }
    }
}
