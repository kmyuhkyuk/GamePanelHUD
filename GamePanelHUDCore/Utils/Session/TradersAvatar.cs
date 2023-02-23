using HarmonyLib;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace GamePanelHUDCore.Utils.Session
{
    public class TradersAvatar
    {
        private static readonly Dictionary<string, Sprite> Avatar = new Dictionary<string, Sprite>();

        public static async void Init(ISession session)
        {
            IList tradersList = Traverse.Create(session).Property("Traders").GetValue<IList>();

            Avatar.Clear();

            foreach (object trader in tradersList)
            {
                object settings = Traverse.Create(trader).Property("Settings").GetValue<object>();

                string id = Traverse.Create(settings).Field("Id").GetValue<string>();

                Sprite sprite = await (Task<Sprite>)Traverse.Create(settings).Method("GetAvatar").GetValue();

                Avatar.Add(id, sprite);
            }
        }

        public static Sprite GetAvatar(string traderid)
        {
            return Avatar.TryGetValue(traderid, out Sprite avatar) ? avatar : null;
        }
    }
}
