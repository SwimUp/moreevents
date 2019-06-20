using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim
{
    public class Dialog
    {
        public DialogDef DialogDef;

        public Dialog_NodeTree Tree;

        public Dictionary<int, DiaNode> Pages = new Dictionary<int, DiaNode>();

        public Action CloseAction;

        public Dialog(DialogDef dialogDef)
        {
            DialogDef = dialogDef;
        }

        public void Init()
        {
            CreatePages();

            CreateOptions();

            Tree = new Dialog_NodeTree(Pages[DialogDef.FirstPageId]);

            Tree.closeAction = CloseAction;
        }

        public void Show()
        {
            Find.WindowStack.Add(Tree);
        }

        private void CreatePages()
        {
            foreach (var page in DialogDef.Pages)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(page.Title);
                builder.Append("\n\n");
                builder.AppendLine(page.Text);

                DiaNode node = new DiaNode(builder.ToString());

                Pages.Add(page.UniqueId, node);
            }
        }

        private void CreateOptions()
        {
            foreach (var page in DialogDef.Pages)
            {
                if (page.Options == null)
                    continue;

                DiaNode node = Pages[page.UniqueId];

                foreach (var option in page.Options)
                {
                    DiaOption opt = new DiaOption(option.Label)
                    {
                        resolveTree = option.DialogEnd
                    };

                    opt.action = option.Action;

                    if (!opt.resolveTree)
                    {
                        opt.link = Pages[option.NextPage];
                    }

                    node.options.Add(opt);
                }
            }
        }
    }
}
