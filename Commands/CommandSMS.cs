﻿using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Diagonal.ZAP
{
    public class CommandSMS : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "sms";

        public string Help => "Send an SMS";

        public string Syntax => " [Player] [Message]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "zap.sms" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer sender = (UnturnedPlayer)caller;

            if (command.Length < 2)
            {
                UnturnedChat.Say(sender, $"Erro! Use: /sms {Syntax}", Color.red);
                return;
            }

            UnturnedPlayer receiver = UnturnedPlayer.FromName(command[0]);

            if (receiver == null)
            {
                UnturnedChat.Say(sender, ZAP.Instance.Translate("no_player"), Color.red);
                return;
            }

            if (receiver.CSteamID == sender.CSteamID)
            {
                UnturnedChat.Say(sender, ZAP.Instance.Translate("no_yourself"), Color.red);
                return;
            }

            string msg = string.Empty;
            int n2 = 0;
            foreach (string msg3 in command)
            {
                n2++;
                if (n2 != 1)
                {
                    msg = msg + " " + msg3;
                }
            }

            if (sender.Experience < ZAP.Instance.Configuration.Instance.SMSCost)
            {
                UnturnedChat.Say(sender, ZAP.Instance.Translate("not_enough_xp"), Color.red);
                return;
            }

            if (ZAP.Instance.Configuration.Instance.SMSEffect)
            {
                sender.TriggerEffect(ZAP.Instance.Configuration.Instance.SendEffectID);
                receiver.TriggerEffect(ZAP.Instance.Configuration.Instance.ReceiveEffectID);
            }

            if (ZAP.sms.ContainsKey(receiver.CSteamID))
                ZAP.sms[receiver.CSteamID] = sender.CSteamID;
            else
                ZAP.sms.Add(receiver.CSteamID, sender.CSteamID);
            
            sender.Experience -= (uint)ZAP.Instance.Configuration.Instance.SMSCost;
            UnturnedChat.Say(sender, ZAP.Instance.Translate("sms_pay", ZAP.Instance.Configuration.Instance.SMSCost), Color.blue);
            UnturnedChat.Say(sender, ZAP.Instance.Translate("send_sms", receiver.DisplayName), ZAP.Instance.Configuration.Instance.ChatColor);
            UnturnedChat.Say(receiver, ZAP.Instance.Translate("receive_sms", sender.DisplayName, msg), ZAP.Instance.Configuration.Instance.ChatColor);
            return;
        }
    }
}
