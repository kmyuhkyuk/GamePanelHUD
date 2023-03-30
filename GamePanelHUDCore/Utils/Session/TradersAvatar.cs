#if !UNITY_EDITOR
using HarmonyLib;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GamePanelHUDCore.Utils.Session
{
    public class TradersAvatar
    {
        private static readonly Dictionary<string, Func<Task<Sprite>>> Avatar = new Dictionary<string, Func<Task<Sprite>>>();

        public static void Init(ISession session)
        {
            IList tradersList = Traverse.Create(session).Property("Traders").GetValue<IList>();

            Avatar.Clear();

            foreach (object trader in tradersList)
            {
                object settings = Traverse.Create(trader).Property("Settings").GetValue<object>();

                string id = Traverse.Create(settings).Field("Id").GetValue<string>();

                Avatar.Add(id, () => { return (Task<Sprite>)Traverse.Create(settings).Method("GetAvatar").GetValue(); });
            }
        }

        public static async Task<Sprite> GetAvatar(string traderid)
        {
            if (Avatar.TryGetValue(traderid, out Func<Task<Sprite>> avatar))
            {
                return await avatar();
            }
            else
            {
                return null;
            }
        }
    }
}
#endif
