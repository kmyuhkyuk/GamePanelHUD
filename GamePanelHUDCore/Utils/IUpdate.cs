#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
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

                    try
                    {
                        if (_removeUpdates.Contains(update))
                        {
                            var num = _removeUpdates.IndexOf(update);

                            _updates.RemoveAt(i);

                            _removeUpdates.RemoveAt(num);
                        }
                        else if (!_stopUpdates.Contains(update))
                        {
                            update.CustomUpdate();
                        }
                    }
                    catch (Exception e)
                    {
                        _updates.RemoveAt(i);

                        LogSource.LogError(e);
                    }
                }
            }
        }
    }
}
#endif