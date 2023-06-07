#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using BepInEx.Logging;

namespace GamePanelHUDCore.Utils
{
    public interface IUpdate
    {
        void CustomUpdate();
    }

    public class UpdateManger
    {
        private readonly List<IUpdate> _updates = new List<IUpdate>();

        private readonly List<IUpdate> _stopUpdates = new List<IUpdate>();

        private readonly List<IUpdate> _removeUpdates = new List<IUpdate>();

        private readonly Debug _debugs = new Debug();

        public bool OutputMethodTime;

        private static readonly ManualLogSource LogSource = Logger.CreateLogSource(nameof(UpdateManger));

        public void Register(IUpdate update)
        {
            if (!_updates.Contains(update))
            {
                _updates.Add(update);
            }
        }

        public void Run(IUpdate update)
        {
            _stopUpdates.Remove(update);
        }

        public void Stop(IUpdate update)
        {
            if (!_stopUpdates.Contains(update))
            {
                _stopUpdates.Add(update);
            }
        }

        public void Remove(IUpdate update)
        {
            if (!_removeUpdates.Contains(update))
            {
                _removeUpdates.Add(update);
            }
        }

        public void Update()
        {
            if (_updates.Count > 0)
            {
                for (var i = 0; i < _updates.Count; i++)
                {
                    var update = _updates[i];

                    if (_removeUpdates.Contains(update))
                    {
                        var num = _removeUpdates.IndexOf(update);

                        _updates.RemoveAt(i);

                        _removeUpdates.RemoveAt(num);
                    }
                    else if (!_stopUpdates.Contains(update))
                    {
                        if (!OutputMethodTime)
                        {
                            update.CustomUpdate();
                        }
                        else
                        {
                            if (i == 0)
                            {
                                LogSource.LogMessage(
                                    $"----------Start----------:CurrentTime:{DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                                _debugs.AllMethodTime.Start();
                            }

                            _debugs.MethodTime.Start();

                            update.CustomUpdate();

                            _debugs.MethodTime.Stop();

                            LogSource.LogMessage($"{update.GetType().Name}:NeedTime:{_debugs.MethodTime.Elapsed}");

                            _debugs.MethodTime.Reset();

                            if (i == _updates.Count - 1)
                            {
                                _debugs.AllMethodTime.Stop();

                                if (_debugs.AllMethodTime.Elapsed > _debugs.MaxTime)
                                {
                                    _debugs.MaxTime = _debugs.AllMethodTime.Elapsed;
                                }
                                else if (_debugs.AllMethodTime.Elapsed < _debugs.MinTime ||
                                         _debugs.MinTime == TimeSpan.Zero)
                                {
                                    _debugs.MinTime = _debugs.AllMethodTime.Elapsed;
                                }

                                LogSource.LogMessage(
                                    $"----------End----------:TotalNeedTime:{_debugs.AllMethodTime.Elapsed}:MaxTime:{_debugs.MaxTime}:MinTime:{_debugs.MinTime}");

                                _debugs.AllMethodTime.Reset();
                            }
                        }
                    }

                    if (!OutputMethodTime)
                    {
                        _debugs.MaxTime = TimeSpan.Zero;
                        _debugs.MinTime = TimeSpan.Zero;
                    }
                }
            }
        }

        public class Debug
        {
            public Stopwatch AllMethodTime = new Stopwatch();

            public Stopwatch MethodTime = new Stopwatch();

            public TimeSpan MaxTime;

            public TimeSpan MinTime;
        }
    }
}
#endif