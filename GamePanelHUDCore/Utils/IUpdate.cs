#if !UNITY_EDITOR
using System;
using System.Text;
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

        public void Register(IUpdate update)
        {
            if (!Updates.Contains(update))
            {
                Updates.Add(update);
            }
        }

        public void Run(IUpdate update)
        {
            if (StopUpdates.Contains(update))
            {
                StopUpdates.Remove(update);
            }
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
                if (!NeedMethodTime)
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
                            update.IUpdate();
                        }
                    }

                    Debugs.MaxTime = TimeSpan.Zero;
                    Debugs.MinTime = TimeSpan.Zero;
                }
                else
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
                            if (i == 0)
                            {
                                GamePanelHUDCorePlugin.LogLogger.LogMessage(Debugs.StringBuilderDatas.Start.StringConcat("----------Start----------:CurrentTime:", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

                                Debugs.AllMethodTime.Start();
                            }

                            Debugs.MethodTime.Start();

                            update.IUpdate();

                            Debugs.MethodTime.Stop();

                            GamePanelHUDCorePlugin.LogLogger.LogMessage(Debugs.StringBuilderDatas.NeedTime.StringConcat(update.GetType().Name, ":NeedTime:", Debugs.MethodTime.Elapsed));

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

                                GamePanelHUDCorePlugin.LogLogger.LogMessage(Debugs.StringBuilderDatas.End.StringConcat("----------End----------:TotalNeedTime:", Debugs.AllMethodTime.Elapsed, ":MaxTime:", Debugs.MaxTime, ":MinTime:", Debugs.MinTime));

                                Debugs.AllMethodTime.Reset();
                            }
                        }
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

            public StringBuilderData StringBuilderDatas = new StringBuilderData();

            public class StringBuilderData
            {
                public StringBuilder Start = new StringBuilder(128);
                public StringBuilder NeedTime = new StringBuilder(128);
                public StringBuilder End = new StringBuilder(128);
            }
        }
    }
}
#endif
