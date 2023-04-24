#if !UNITY_EDITOR
using BepInEx.Logging;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace GamePanelHUDCore.Utils
{
    public interface IUpdate
    {
        void IUpdate();
    }

    public class IUpdateManger
    {
        private readonly List<IUpdate> Updates = new List<IUpdate>();

        private readonly List<IUpdate> StopUpdates = new List<IUpdate>();

        private readonly List<IUpdate> RemoveUpdates = new List<IUpdate>();

        private readonly Debug Debugs = new Debug();

        public bool NeedMethodTime;

        private static readonly ManualLogSource LogSource = Logger.CreateLogSource("IUpdateManger");

        public void Register(IUpdate update)
        {
            if (!Updates.Contains(update))
            {
                Updates.Add(update);
            }
        }

        public void Run(IUpdate update)
        {
            StopUpdates.Remove(update);
        }

        public void Stop(IUpdate update)
        {
            if (!StopUpdates.Contains(update))
            {
                StopUpdates.Add(update);
            }
        }

        public void Remove(IUpdate update)
        {
            if (!RemoveUpdates.Contains(update))
            {
                RemoveUpdates.Add(update);
            }
        }

        public void Update()
        {
            if (Updates.Count > 0)
            {
                for (int i = 0; i < Updates.Count; i++)
                {
                    IUpdate update = Updates[i];

                    if (RemoveUpdates.Contains(update))
                    {
                        int num = RemoveUpdates.IndexOf(update);

                        Updates.RemoveAt(i);

                        RemoveUpdates.RemoveAt(num);
                    }
                    else if (!StopUpdates.Contains(update))
                    {
                        if (!NeedMethodTime)
                        {
                            update.IUpdate();
                        }
                        else
                        {
                            if (i == 0)
                            {
                                LogSource.LogMessage(string.Concat("----------Start----------:CurrentTime:", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

                                Debugs.AllMethodTime.Start();
                            }

                            Debugs.MethodTime.Start();

                            update.IUpdate();

                            Debugs.MethodTime.Stop();

                            LogSource.LogMessage(string.Concat(update.GetType().Name, ":NeedTime:", Debugs.MethodTime.Elapsed));

                            Debugs.MethodTime.Reset();

                            if (i == Updates.Count - 1)
                            {
                                Debugs.AllMethodTime.Stop();

                                if (Debugs.AllMethodTime.Elapsed > Debugs.MaxTime)
                                {
                                    Debugs.MaxTime = Debugs.AllMethodTime.Elapsed;
                                }
                                else if (Debugs.AllMethodTime.Elapsed < Debugs.MinTime || Debugs.MinTime == TimeSpan.Zero)
                                {
                                    Debugs.MinTime = Debugs.AllMethodTime.Elapsed;
                                }

                                LogSource.LogMessage(string.Concat("----------End----------:TotalNeedTime:", Debugs.AllMethodTime.Elapsed, ":MaxTime:", Debugs.MaxTime, ":MinTime:", Debugs.MinTime));

                                Debugs.AllMethodTime.Reset();
                            }
                        }

                        update.IUpdate();
                    }

                    if (!NeedMethodTime)
                    {
                        Debugs.MaxTime = TimeSpan.Zero;
                        Debugs.MinTime = TimeSpan.Zero;
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
