#if !UNITY_EDITOR
using HarmonyLib;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace GamePanelHUDCore.Utils.Session
{
    public static class TradersAvatar
    {
        private static readonly Dictionary<string, object> Avatar = new Dictionary<string, object>();

        private static readonly Dictionary<string, Task<Sprite>> AvatarSprites = new Dictionary<string, Task<Sprite>>();

        public static void Init(ISession session)
        {
            IList tradersList = Traverse.Create(session).Property("Traders").GetValue<IList>();

            Avatar.Clear();
            AvatarSprites.Clear();

            foreach (object trader in tradersList)
            {
                object settings = Traverse.Create(trader).Property("Settings").GetValue<object>();

                string id = Traverse.Create(settings).Field("Id").GetValue<string>();

                Avatar.Add(id, settings);
            }
        }

        public static async Task<Sprite> GetAvatar(string traderId)
        {
            if (AvatarSprites.TryGetValue(traderId, out Task<Sprite> sprite))
            {
                return await sprite;
            }
            else
            {
                if (Avatar.TryGetValue(traderId, out object avatar))
                {
                    Task<Sprite> avatarSprite = Traverse.Create(avatar).Method("GetAvatar").GetValue<Task<Sprite>>();
                    AvatarSprites.Add(traderId, avatarSprite);

                    return await avatarSprite;
                }
                else
                {
                    return null;
                }
            }
        }

        public static async void GetAvatar(string traderId, Action<Sprite> action)
        {
            var sprite = await GetAvatar(traderId);

            if (action != null)
            {
                action(sprite);
            }
        }
    }
}
#endif
