using _SGUI_.context_click;

namespace _COBALT_
{
    partial class SguiNotepad
    {
        void InitHeader_File()
        {
            var button_file = AddHeaderButton();
            button_file.transform.SetSiblingIndex(1);

            button_file.trad.SetTrads(new()
            {
                french = "Fichier",
                english = "File",
            });

            button_file.onContextList += (ContextList list) =>
            {
                {
                    var button = list.AddButton();
                    button.trad.SetTrads(new()
                    {
                        french = "Nouveau Fichier",
                        english = "New File",
                    });
                }

                {
                    var button = list.AddButton();
                    button.trad.SetTrads(new()
                    {
                        french = "Ouvrir Fichier",
                        english = "Open File",
                    });
                }

                {
                    var button = list.AddButton();
                    button.trad.SetTrads(new()
                    {
                        french = "Ouvrir Dossier",
                        english = "Open Folder",
                    });
                }

                {
                    var button = list.AddButton();
                    button.trad.SetTrads(new()
                    {
                        french = "Sauvegarder",
                        english = "Save",
                    });
                }

                {
                    var button = list.AddButton();
                    button.trad.SetTrads(new()
                    {
                        french = "Sauvegarder Sous",
                        english = "Save As",
                    });

                    button.SetupSublist(sublist =>
                    {
                        {
                            var button = sublist.AddButton();
                            button.trad.SetTrads(new()
                            {
                                french = "Autre Format",
                                english = "Other format",
                            });
                        }
                    });
                }
            };
        }
    }
}