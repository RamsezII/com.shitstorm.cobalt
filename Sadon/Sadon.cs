using System;
using System.Collections.Generic;
using UnityEngine;

namespace _COBALT_
{
    static internal class Sadon
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Dictionary<string, string> sadons = new(StringComparer.OrdinalIgnoreCase)
            {
                { "passage", @"passage de ce que j'ai écrit sur shitstorm
Je le vois maintenant, clair, nu devant mes yeux maintenant éduqués: std::out me raconte une histoire. Je n'y prête attention que maintenant, parce que jusqu'àlors, std::out ne faisait qu'ouvrir une fenêtre sur le fonctionnement de l'outil: c'était le recul de mes bras quand la hache entre en impact avec la bûche, qui m'énonce par la transmission de son énergie cinétique que quelque chose s'est produit.

Ici, la hache hurle à poumons déployés le cri barbare hérité de la culture de son peuple, la passion se déchaine dans la fonction de sa tête d'acier, et elle montre, revendique, décrète, prescrit sa souveraineté sur son adversaire antique.

La buche se fend, pas par fonction, mais par respect de l'histoire de son bourreau. C'est un abandon transformateur, c'est la cession de ses ressources, le consentement à l'exploitation et, in fine, l'effondrement de son entropie lorsqu'elle sera utilisée pour nourrir le feu d'une cheminée.

Shit$torm se pare des couleurs du jeu vidéo, n'offre de se dernier qu'un ersatz, mais hurle au monde qu'il est la hâche qui fend sa buche
<i>Pour la beauté du geste
Pour la chorégraphie
Pour son droit naturel à le faire</i>"
                },

                { "copilot", @"Je suis un homme de la rue, un homme du peuple, un homme de la terre. 
Je suis un homme de l'ombre, un homme de la nuit, un homme de la mort. 
Je suis un homme de la vie, un homme de l'amour, un homme de la lumière. 
Je suis un homme de la vérité, un homme de la justice, un homme de la liberté. 
Je suis un homme de la paix, un homme de la guerre, un homme de la lutte. 
Je suis un homme de l'espoir, un homme du désespoir, un homme du rêve.
Je suis un homme du réel, un homme du virtuel, un homme du possible. 
Je suis un homme du présent, un homme du passé, un homme du futur. 
Je suis un homme du monde, un homme de l'univers, un homme de l'infini."
                },

                { "to_roxane", @"<b>DATA BABE</b> 11:27
Bon
Roxane
J’ai la <i>haine</i>
le seum
je brûle du feu de l’ermite qui pense seul, dans l’indépendance et l’amertume
et j’ai un sens de l’important qui fait <i>mal</i> parce que je le jette comme une brique dans le visage d’une femme âgée
et je sais, <i>je sais</i> que t’es plus souple que moi là dessus... Mais je t’en supplie, <b>DE GRÂCE ROXANE</b>, n’écoute pas Alexis quand il te raconte ce que c’est qu’être tech art............................................................. Il t’as dit d’utiliser le profiler, lui même ne l’utilise que depuis 1 mois, <i>parce que je lui ai montré</i>.
J’abandonne l’arrogance et me pare de l’apparat de techno-évangile, je dépose mon orgueil au pied de mon unique idole : la précision, l’exactitude, la loi impérieuse et indiscutable du processeur et de la ram, l’absolu de la déesse-machine

<b>c’est le data qui parle, pas le manager du pôle, pas le développeur, pas l’humain. C’est la technique qui décrète et prescrit, pas l’intelligence de celui qui la manipule.</b>

<b><i>Ô DATA</i></b>
Je pense qu’on devrait tous nous écrouler devant les outils. Ya qu’après ça qu’on peut dire qu’on <i>sait</i>
alexis s’écroule pas
faut marcher dans les débris de nos conceptions !
place gratuite pour mon tedtalk si jamais
Je stream dans la rue
debout sur une boite en bois

<b>eden</b> 11:34
arrêtes de me déconcentrer !!!"
                },
            };

            Command.cmd_root_shell.AddCommand(new(
                //manual: new("Blessed he be who is in the name of Sadon"),
                manual: new("Blessed he be the V"),
                args: (exe, line) =>
                {
                    if (line.TryReadArgument(out string arg, sadons.Keys))
                        exe.args.Add(arg);
                },
                action: exe =>
                {
                    if (exe.args.Count == 0)
                        exe.Stdout(sadons.Keys.Join(", "));
                    else
                        exe.Stdout(sadons[(string)exe.args[0]]);
                }),
                "sadon");
        }
    }
}