using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class Terminal : Window
    {
        public TerminalDef Def;

        private Vector2 terminalTextScroll = Vector2.zero;

        public override Vector2 InitialSize => new Vector2(600, 400);

        private static readonly Color terminalTextColor = new ColorInt(75, 204, 0).ToColor;
        private readonly Color terminalColor = new ColorInt(0, 0, 0).ToColor;
        private readonly Color borderColor = new ColorInt(189, 189, 189).ToColor;

        private static GUIStyle textStyle;

        public string TerminalCommand = string.Empty;
        public string TerminalText = string.Empty;
        public StringBuilder TerminalTextBuilder = new StringBuilder();

        public Dictionary<string, TerminalCommand> Commands;
        protected override float Margin => 0f;

        protected string InitialText => Def.InitialText;

        static Terminal()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = terminalTextColor;

            textStyle = style;
        }

        public Terminal(TerminalDef terminalDef)
        {
            Commands = new Dictionary<string, TerminalCommand>();

            Def = terminalDef;

            TerminalText = InitialText;
            TerminalTextBuilder.Append(TerminalText);

            foreach (var command in terminalDef.Commands)
            {
                Commands.Add(command.CommandKey, command);
                command.Terminal = this;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;

            GUI.color = borderColor;
            Widgets.DrawBox(inRect, 3);
            Rect solidRect = new Rect(6, 4, 589, 360);
            Widgets.DrawBoxSolid(solidRect, terminalColor);
            GUI.color = terminalTextColor;
            Rect labelRect = new Rect(6, 4, 589, 360);
            Widgets.LabelScrollable(labelRect, TerminalText, ref terminalTextScroll);
            GUI.color = Color.white;
            TerminalCommand = GUI.TextField(new Rect(5, inRect.yMax - 30, 591, 30), TerminalCommand, textStyle);
        }

        public override void OnAcceptKeyPressed()
        {
            PreFlush();
        }

        protected virtual void PreFlush()
        {
            Flush();
        }

        protected virtual void Flush(bool scroll = true)
        {
            string text = $"\n--> {TerminalCommand}";
            TerminalTextBuilder.Append(text);

            string mainCommand = TerminalCommand.Split(' ')[0];
            switch(mainCommand)
            {
                case "/help":
                    {
                        HelpCommand();
                        break;
                    }
                case "/clear":
                    {
                        Clear();
                        break;
                    }
                case "/exit":
                    {
                        ExitCommand();
                        break;
                    }
                default:
                    {
                        if (Commands.ContainsKey(mainCommand))
                        {
                            TerminalTextBuilder.Append("\n");
                            TerminalTextBuilder.Append(Commands[mainCommand].Invoke());
                        }
                        else
                        {
                            TerminalTextBuilder.Append(": Unknown command");
                        }
                        break;
                    }
            }

            TerminalText = TerminalTextBuilder.ToString();
            TerminalCommand = string.Empty;

            if(scroll)
            {
                Scroll(Text.CalcHeight(TerminalText, 589));
            }
        }

        protected virtual void ExitCommand()
        {

        }

        public void Scroll(float offset)
        {
            terminalTextScroll.y += offset;
        }

        public void HelpCommand()
        {
            TerminalTextBuilder.Append("\nCommands list\n");
            TerminalTextBuilder.AppendLine($"--> /help - {"HelpCommand".Translate()}");
            TerminalTextBuilder.AppendLine($"--> /clear - {"ClearCommand".Translate()}");
            foreach (var command in Commands)
            {
                if(command.Value.ShowInHelp)
                    TerminalTextBuilder.AppendLine($"--> {command.Key} - {command.Value.CommandDescription}");
            }
            TerminalTextBuilder.Append("===========================\n");

            TerminalText = TerminalTextBuilder.ToString();
        }

        public void Clear()
        {
            TerminalTextBuilder.Length = 0;
            TerminalText = "";
        }
    }
}
