using _SGUI_.context_click;

namespace _COBALT_
{
    partial class SguiNotepad
    {
        void InitHeader_File()
        {
            var button_file = AddHeaderButton(new()
            {
                french = "Fichier",
                english = "File",
            });
            button_file.transform.SetSiblingIndex(0);

            button_file.onContextList += (ContextList list) =>
            {
                {
                    var button = list.AddButton(new()
                    {
                        french = "Nouveau fichier",
                        english = "New file",
                    });
                }

                {
                    var button = list.AddButton(new()
                    {
                        french = "Ouvrir fichier",
                        english = "Open file",
                    });
                }

                {
                    var button = list.AddButton(new()
                    {
                        french = "Ouvrir dossier",
                        english = "Open folder",
                    });
                }

                {
                    var button = list.AddButton(new()
                    {
                        french = "Sauvegarder",
                        english = "Save",
                    });
                }

                {
                    var button = list.AddButton(new()
                    {
                        french = "Sauvegarder sous",
                        english = "Save as",
                    });

                    button.SetupSublist(sublist =>
                    {
                        {
                            var button = sublist.AddButton(new()
                            {
                                french = "Autre format",
                                english = "Other format",
                            });
                        }
                    });
                }
            };
        }
    }
}