using HarmonyLib;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace GamePanelHUDCore.Utils.Session
{
    public class TradersAvatar
    {
        public static readonly Dictionary<string, Sprite> Avatar = new Dictionary<string, Sprite>();

        public static async void Init(object session)
        {
            IList tradersList = Traverse.Create(session).Property("Traders").GetValue<object>() as IList;

            foreach (object trader in tradersList)
            {
                object settings = Traverse.Create(trader).Property("Settings").GetValue<object>();

                string id = Traverse.Create(settings).Field("Id").GetValue<string>();

                Sprite sprite = await (Task<Sprite>)Traverse.Create(settings).Method("GetAvatar").GetValue();

                Avatar.Add(id, sprite);
            }
        }
    }
}
