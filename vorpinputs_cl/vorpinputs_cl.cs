﻿using System;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpinputs_cl
{
    public class vorpinputs_cl : BaseScript
    {
        private static string text;

        public vorpinputs_cl()
        {
            API.RegisterCommand("closeinput", new Action(CloseInput), false);

            EventHandlers["vorpinputs:getInput"] += new Action<string, string, dynamic>(getInputs);

            API.RegisterNuiCallbackType("submit");
            EventHandlers["__cfx_nui:submit"] += new Action<ExpandoObject>(SetSubmit);

            API.RegisterNuiCallbackType("close");
            EventHandlers["__cfx_nui:close"] += new Action<ExpandoObject>(SetClose);
        }

        private void SetClose(dynamic result)
        {
            text = result.stringtext;
        }

        private void SetSubmit(dynamic result)
        {
            text = result.stringtext;
        }

        private void getInputs(string title, string placeholder, dynamic cb)
        {
            WaitToInputs(title, placeholder, cb);
        }

        public async Task WaitToInputs(string button, string placeholder, dynamic cb)
        {
            API.SetNuiFocus(true, true);
            var json = "{\"type\": \"enableinput\",\"style\": \"block\",\"button\": \"" + button +
                       "\",\"placeholder\": \"" + placeholder + "\"}";
            API.SendNuiMessage(json);

            while (text == null)
            {
                await Delay(1);
            }

            cb.Invoke(text);

            await Delay(1);
            text = null;
            CloseInput();
        }

        private void CloseInput()
        {
            API.SetNuiFocus(false, false);

            var json = "{\"type\": \"enableinput\",\"style\": \"none\"}";

            API.SendNuiMessage(json);
        }
    }
}
