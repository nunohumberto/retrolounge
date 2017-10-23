using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HtmlAgilityPack;
using System.Xml;
using Microsoft.AspNet.Identity;

namespace EDCFinal
{
    public partial class Populate : System.Web.UI.Page
    {

        public Tuple<string, string, string, string> scrapGameData(string url, string console)      // Obter toda a informação possível de um jogo
        {
            string image = "";
            string releasedate = "";
            string developers = "";
            string publishers = "";
            if (console.Equals("NES") || console.Equals("GBC") || console.Equals("GBA") || console.Equals("GB"))
            {
                HtmlDocument basepage = new HtmlAgilityPack.HtmlDocument();

                WebClient cli = new WebClient();
                cli.Encoding = System.Text.Encoding.UTF8;
                basepage.LoadHtml(cli.DownloadString("http://nintendo.wikia.com" + url));

                HtmlNodeCollection rows = basepage.DocumentNode.SelectNodes("//table[@class='infobox']/tr");

                foreach (HtmlNode row in rows)      // Deteção de campos Developer e Publisher
                {
                    HtmlNodeCollection subnodes = row.ChildNodes;
                    if (subnodes[1].InnerText.Contains("Developer(s)"))
                    {
                        foreach (HtmlNode subsubnode in subnodes[2].ChildNodes)
                        {
                            if (subsubnode.Name == "a") developers += (subsubnode.InnerText.Trim() + ",");
                        }
                    }
                    if (subnodes[1].InnerText.Contains("Publisher(s)"))
                    {
                        foreach (HtmlNode subsubnode in subnodes[2].ChildNodes)
                        {
                            if (subsubnode.Name == "a") publishers += (subsubnode.InnerText.Trim() + ",");
                        }
                    }

                }

                foreach (HtmlNode row in rows)
                {
                    Boolean found = false;
                    HtmlNodeCollection subnodes = row.ChildNodes;
                    foreach (HtmlNode descendant in subnodes.Descendants())     // Deteção de um campo de imagem de capa
                    {
                        if (descendant.Name == "img")
                        {
                            foreach (HtmlAttribute attr in descendant.Attributes)
                            {
                                if (attr.Name == "src") System.Diagnostics.Debug.WriteLine(attr.Value);
                                image = attr.Value;
                                found = true;
                                break;
                            }
                            if (found) break;
                        }
                    }
                    if (found) break;
                }

                HtmlNodeCollection release_elements = basepage.DocumentNode.SelectNodes("//table[@class='infobox']/tr");

                foreach (HtmlNode row in release_elements)
                {
                    Boolean found = false;
                    HtmlNodeCollection subnodes = row.ChildNodes;
                    foreach (HtmlNode descendant in subnodes.Descendants())     // Deteção de um campo de data
                    {
                        if (descendant.InnerText.Contains("Release Date(s)"))
                        {
                            foreach (HtmlNode subdescendant in descendant.Descendants())
                            {
                                if (subdescendant.Name == "td" &&
                                    !subdescendant.InnerText.ToUpper().Contains("NA:") && !subdescendant.InnerText.ToUpper().Contains("JP:") &&
                                    !subdescendant.InnerText.ToUpper().Contains("EU:") && !subdescendant.InnerText.ToUpper().Contains("AU:") && (
                                    subdescendant.InnerText.ToUpper().Contains("JANUARY") || subdescendant.InnerText.ToUpper().Contains("FEBRUARY") ||
                                    subdescendant.InnerText.ToUpper().Contains("MARCH") || subdescendant.InnerText.ToUpper().Contains("APRIL") ||
                                    subdescendant.InnerText.ToUpper().Contains("MAY") || subdescendant.InnerText.ToUpper().Contains("JUNE") ||
                                    subdescendant.InnerText.ToUpper().Contains("JULY") || subdescendant.InnerText.ToUpper().Contains("AUGUST") ||
                                    subdescendant.InnerText.ToUpper().Contains("SEPTEMBER") || subdescendant.InnerText.ToUpper().Contains("OCTOBER") ||
                                    subdescendant.InnerText.ToUpper().Contains("NOVEMBER") || subdescendant.InnerText.ToUpper().Contains("DECEMBER")))


                                {
                                    releasedate = subdescendant.InnerText;
                                    found = true;
                                    break;
                                }
                            }
                            if (found) break;
                        }
                    }
                    if (found) break;
                }



            }

            else
            {

               // SEGA wiki went down! :(
            }



            return Tuple.Create(image, releasedate, developers, publishers);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if(Context.User.Identity.GetUserId() != "190d22ec-44a8-4bf4-9579-0365a6b5af3e")
            {
                Response.Redirect("/Publib");
                return;
            }

            string CONSOLE;
            string qs = "";
            try
            {
                qs = Request.QueryString["c"];
                CONSOLE = qs;
            }
            catch (Exception)
            {
                CONSOLE = "GBC";
                return;
            }


            HtmlDocument basepage = new HtmlAgilityPack.HtmlDocument();
            WebClient cli = new WebClient();
            cli.Encoding = System.Text.Encoding.UTF8;       // Atenção ao encoding!
            if (CONSOLE.Equals("NES")) basepage.LoadHtml(cli.DownloadString("http://nintendo.wikia.com/wiki/List_of_Nintendo_Entertainment_System_games"));     // Seleção de consola
            else if (CONSOLE.Equals("GB")) basepage.LoadHtml(cli.DownloadString("http://nintendo.wikia.com/wiki/List_of_Game_Boy_games"));
            else if (CONSOLE.Equals("GBC")) basepage.LoadHtml(cli.DownloadString("http://nintendo.wikia.com/wiki/List_of_Game_Boy_Color_games"));
            else if (CONSOLE.Equals("GBA")) basepage.LoadHtml(cli.DownloadString("http://nintendo.wikia.com/wiki/List_of_Game_Boy_Advance_games"));
            else if (CONSOLE.Equals("SMS")) basepage.LoadHtml(cli.DownloadString("http://segaretro.org/List_of_Master_System_games"));

            int count = 0;
            List<string> gametitles = new List<string>();

            if (CONSOLE.Equals("NES") || CONSOLE.Equals("GBC") || CONSOLE.Equals("GB") || CONSOLE.Equals("GBA")) {          // Se for selecionada uma consola da nintendo, então utilizar a fonte nintendo.wikia.com
                foreach (HtmlNode game_table in basepage.DocumentNode.SelectNodes("//table[@class='wikitable sortable']/tr")) { // Percorrer todas as linhas das tabelas
                    HtmlNodeCollection nodes = game_table.ChildNodes;
                    string gametitle = HttpUtility.HtmlDecode(nodes[1].InnerText);
                    System.Diagnostics.Debug.WriteLine(gametitle);
                    string gameurl = "";
                    int gameurl_len = 0;
                    Boolean updated = false;

                    HtmlAttributeCollection attribs = (nodes[1].ChildNodes)[0].Attributes;
                    foreach (HtmlAttribute attrib in attribs)
                    {
                        if (attrib.Name.Equals("href"))
                        {
                            gameurl = attrib.Value;
                            gameurl_len = gameurl.Length;
                            break;
                        }
                    }

                    if (gameurl_len > 8 && gameurl.Substring(gameurl_len - 9).Equals("redlink=1")) continue; // Ignorar se se tratar de um redlink (página não existente na wiki)
                    if (gameurl.Equals("")) continue;
                    if (gametitles.Contains(gametitle.Replace('é', 'e'))) continue;
                    gametitles.Add(gametitle.Replace('é', 'e'));

                    List<string> genres = new List<string>();

                    try
                    {
                        genres = fetchGenre(gameurl, "nintendo");       // Obter o género
                        updated = true;
                    }
                    catch (Exception)
                    { }


                    if (!updated)               // Caso não tenha sido possível, então iremos alterar ligeiramente o URL
                    {
                        try
                        {
                            if (CONSOLE.Equals("NES"))
                            {
                                genres = fetchGenre(gameurl + "_(NES)", "nintendo");
                                gameurl = gameurl + "_(NES)";
                            }
                            else if (CONSOLE.Equals("GBC"))
                            {
                                genres = fetchGenre(gameurl + "_(GBC)", "nintendo");
                                gameurl = gameurl + "_(GBC)";
                            }
                            else if (CONSOLE.Equals("GB"))
                            {
                                genres = fetchGenre(gameurl + "_(GB)", "nintendo");
                                gameurl = gameurl + "_(GB)";
                            }
                            else if (CONSOLE.Equals("GBA"))
                            {
                                genres = fetchGenre(gameurl + "_(GBA)", "nintendo");
                                gameurl = gameurl + "_(GBA)";
                            }
                            updated = true;
                        }
                        catch (Exception) { }

                    }

                    if (!updated)        // Caso não tenha sido possível, então iremos alterar novamente o URL
                    {
                        try
                        {
                            if (CONSOLE.Equals("NES"))
                            {
                                genres = fetchGenre(gameurl + "_(Famicom)", "nintendo");
                                gameurl = gameurl + "_(Famicom)";
                            }
                            else if (CONSOLE.Equals("GB"))
                            {
                                genres = fetchGenre(gameurl + "_(Game_Boy)", "nintendo");
                                gameurl = gameurl + "_(Game_Boy)";
                            }
                            else if (CONSOLE.Equals("GBC"))
                            {
                                genres = fetchGenre(gameurl + "_(Game_Boy_Color)", "nintendo");
                                gameurl = gameurl + "_(Game_Boy_Color)";
                            }
                            else if (CONSOLE.Equals("GBA"))
                            {
                                genres = fetchGenre(gameurl + "_(Game_Boy_Advance)", "nintendo");
                                gameurl = gameurl + "_(Game_Boy_Advance)";
                            }
                            updated = true;
                        }
                        catch (Exception) { }

                    }

                    if (!updated)               // e novamente
                    {
                        try
                        {
                            genres = fetchGenre(gameurl + "_(video_game)", "nintendo");
                            gameurl = gameurl + "_(video_game)";
                            updated = true;
                        }
                        catch (Exception) { }

                    }

                    if (!updated)
                    {
                        try
                        {
                            genres = fetchGenre(gameurl + "_(game)", "nintendo");
                            gameurl = gameurl + "_(game)";
                            updated = true;
                        }
                        catch (Exception) { }

                    }




                    String genrestring;
                    if (!updated) continue;     // Se não tiver sido encontrada informação, passar para o próximo elemento da tabela.
                    else if (genres.Count == 1 && genres[0].Equals("") || (genres.Count == 0 && CONSOLE.Equals("GB")) ) genrestring = "unavailable";
                    else {
                        genrestring = "";
                        foreach (string g in genres)
                        {
                            genrestring += (g + " & ");
                        }
                        genrestring = genrestring.Remove(genrestring.Length - 3);
                    }


                    gametitle = fixGameTitle(gametitle);

                    if (CONSOLE.Equals("NES"))  // Lista de jogos da NES cujo formato dos metadados não respeita o regular
                    {
                        if (gameurl.Equals("/wiki/Mother") || gameurl.Equals("/wiki/Dark_Lord") || gameurl.Equals("/wiki/Tecmo_Super_Bowl") || gameurl.Equals("/wiki/DuckTales_2")
                            || gameurl.Equals("/wiki/Monster_Party") || gameurl.Equals("/wiki/Bigfoot") || gameurl.Equals("/wiki/Snake%27s_Revenge") || gameurl.Equals("/wiki/The_Simpsons:_Bart_vs._the_Space_Mutants")
                            || gameurl.Equals("/wiki/Blackjack") 
                            ) continue;
                    }

                    else if (CONSOLE.Equals("GB")) // Lista de jogos de Gameboy cujo formato dos metadados não respeita o regular
                    {
                        if (gameurl.Equals("/wiki/Tetris_(Game_Boy)") || gameurl.Equals("/wiki/Game_Boy_Camera") || gameurl.Equals("/wiki/Who_Framed_Roger_Rabbit")
                         || gameurl.Equals("/wiki/Exodus:_Journey_to_the_Promised_Land") || gameurl.Equals("/wiki/Star_Trek:_25th_Anniversary") || gameurl.Equals("/wiki/Swamp_Thing")
                         || gameurl.Equals("/wiki/Disney%27s_The_Little_Mermaid") || gameurl.Equals("/wiki/Robin_Hood:_Prince_of_Thieves") || gameurl.Equals("/wiki/Top_Rank_Tennis")
                         || gameurl.Equals("/wiki/Earthworm_Jim_(handheld)") || gameurl.Equals("/wiki/NHL_Hockey_%2795") || gameurl.Equals("/wiki/Super_Star_Wars:_Return_of_the_Jedi")
                         || gameurl.Equals("/wiki/NHL_96_(Game_Boy)") || gameurl.Equals("/wiki/Pinocchio") || gameurl.Equals("/wiki/Popeye") || gameurl.Equals("/wiki/Hatris")
                         || gameurl.Equals("/wiki/R-Type") || gameurl.Equals("/wiki/World_Ice_Hockey") || gameurl.Equals("/wiki/Spy_vs._Spy") || gameurl.Equals("/wiki/Alfred_Chicken")
                         || gameurl.Equals("/wiki/Pocket_Bomberman")) continue;
                    }


                    else if (CONSOLE.Equals("GBC")) // Lista de jogos de Gameboy Color cujo formato dos metadados não respeita o regular
                    {
                        if (gameurl.Equals("/wiki/Hugo") || gameurl.Equals("/wiki/Scrabble") || gameurl.Equals("/wiki/Checkmate")) continue;
                        if (gameurl.Equals("/wiki/Rayman")) gameurl = "/wiki/Rayman_(video_game)";
                    }

                    else if (CONSOLE.Equals("GBA")) // Lista de jogos de Gameboy Advance cujo formato dos metadados não respeita o regular
                    {
                        if (gameurl.Equals("/wiki/Golden_Sun") || gameurl.Equals("/wiki/NFL_Blitz_20-03") || gameurl.Equals("/wiki/Rocky")
                         || gameurl.Equals("/wiki/Wade_Hixton%27s_Counter_Punch") || gameurl.Equals("/wiki/The_Ant_Bully")) continue;
                    }


                    addtoDB(gametitle.Trim(), gameurl, genres, CONSOLE);
                    Tuple<string, string, string, string> gamedata = scrapGameData(gameurl, CONSOLE);
                    gamedata = fixExceptions(gamedata, gametitle, gameurl);

                    datacontainer.InnerHtml += "<tr><td>" + gametitle + "</td><td>" + gameurl + "</td><td>" + genrestring + "</td><td>" + gamedata.Item3 + "</td><td>" + gamedata.Item4 + "</td><td>" + gamedata.Item2 + "</td><td><img src=\"" + gamedata.Item1 + "\"/></td></tr>";
                    
                    count++;
                }
            }
            else if (CONSOLE.Equals("SMS"))             // Obter lista de jogos da Sega Master System. Fonte: segaretro.org
            {
                foreach (HtmlNode game_table in basepage.DocumentNode.SelectNodes("//table[@class='prettytable sortable'][1]/tr[position()>1]"))
                {
                    string gameurl = "";
                    string gametitle = "";

                    HtmlNodeCollection nodes = game_table.ChildNodes[1].ChildNodes;
                    foreach (HtmlNode node in nodes)
                    {
                        HtmlNodeCollection subnodes = node.ChildNodes;
                        foreach (HtmlNode subnode in subnodes)
                        {
                            foreach (HtmlAttribute attr in subnode.Attributes)
                            {
                                if (attr.Name.Equals("href")) {
                                    gameurl = attr.Value;
                                }
                            }
                            gametitle = subnode.InnerText;
                        }
                    }
                    List<string> genres = fetchGenre(gameurl, "sega");

                    String genrestring;
                    if (genres.Count == 1 && genres[0].Equals("")) genrestring = "unavailable";
                    else {
                        genrestring = "";
                        foreach (string g in genres)
                        {
                            genrestring += (g + " & ");
                        }
                        genrestring = genrestring.Remove(genrestring.Length - 3);
                    }

                    //addtoDB(gametitle.Trim(), gameurl, genres, CONSOLE);
                    datacontainer.InnerHtml += "<tr><td>" + gametitle + "</td><td>" + gameurl + "</td><td>" + genrestring + "</td></tr>";
                }
            }
            System.Diagnostics.Debug.WriteLine("Scraped " + count.ToString() + " games.");
        }

        public Tuple<string, string, string, string> fixExceptions(Tuple<string, string, string, string> gamedata, string gametitle, string gameurl)
        {   // Ajuste de exceções de jogos cuja imagem ou formato da data fogem ao normal
            string chosen_image = gamedata.Item1;
            if (gametitle.Contains("LeafGreen")) chosen_image = "http://vignette2.wikia.nocookie.net/nintendo/images/5/5a/Pokemon_LeafGreen_%28NA%29.png/revision/latest/scale-to-width-down/250?cb=20121123224250&path-prefix=en";
            if (gametitle.Contains("Pokémon Silver")) chosen_image = "http://vignette2.wikia.nocookie.net/nintendo/images/3/37/Pokemon_Silver_%28NA%29.jpg/revision/latest/scale-to-width-down/250?cb=20111104144755&path-prefix=en";
            if (gametitle.Contains("Pokemon Sapphire")) chosen_image = "http://vignette2.wikia.nocookie.net/nintendo/images/c/cb/Pokemon_Sapphire_%28NA%29.jpg/revision/latest/scale-to-width-down/250?cb=20140822030107&path-prefix=en";
            if (gametitle.Contains("Pokémon Blue")) chosen_image = "http://vignette3.wikia.nocookie.net/nintendo/images/6/60/Pokemon_Blue_%28NA%29.png/revision/latest/scale-to-width-down/250?cb=20120331144905&path-prefix=en";

            string chosen_date = gamedata.Item2.Trim();
            if (chosen_date.EndsWith("]")) chosen_date = chosen_date.Remove(chosen_date.Length - 3);

            if (gameurl.Equals("/wiki/Wario_Land_II")) chosen_date = "March 2, 1998";  // some exceptions
            else if (gameurl.Equals("/wiki/Balloon_Fight_GB")) chosen_date = "July 31, 2000";
            else if (gameurl.Equals("/wiki/Harry_Potter_and_the_Chamber_of_Secrets")) chosen_date = "November 14, 2002";
            else if (gameurl.Equals("/wiki/Super_Mario_Advance")) chosen_date = "June 11, 2001";
            else if (gameurl.Equals("/wiki/Mario_%26_Luigi:_Superstar_Saga")) chosen_date = "November 17, 2003";
            else if (gameurl.Equals("/wiki/Donkey_Kong_Country_2:_Diddy%27s_Kong_Quest")) chosen_date = "December 1995";
            else if (gameurl.Equals("/wiki/Donkey_Kong_Country_3")) chosen_date = "November 22, 1996";
            else if (gameurl.Equals("/wiki/F-1_Race_(Game_Boy)")) chosen_date = "February 1991";
            else if (gameurl.Equals("/wiki/Bonk%27s_Revenge")) chosen_date = "July 9, 1991";
            else if (gameurl.Equals("/wiki/Donkey_Kong_Land_2")) chosen_date = "September 1996";
            else if (gameurl.Equals("/wiki/Castlevania:_The_Adventure")) chosen_date = "December 1989";

            return Tuple.Create(chosen_image, chosen_date, gamedata.Item3, gamedata.Item4);
        }

        public void addtoDB(string title, string url, List<string> genres, string console)  // Adiciona um jogo à base de dados 
        {   
            XmlDocument newgame_doc = new System.Xml.XmlDocument();
            XmlElement game = newgame_doc.CreateElement("game");
            XmlElement genres_toadd = newgame_doc.CreateElement("genres");


            if (genres.Count == 0 || (genres.Count == 1 && genres[0].Equals(""))) {
                XmlElement newgenre = newgame_doc.CreateElement("genre");
                XmlAttribute newgenre_att = newgame_doc.CreateAttribute("name");
                newgenre_att.InnerText = "unavailable";
                newgenre.Attributes.Append(newgenre_att);
                genres_toadd.AppendChild(newgenre);
            }
            else {
                foreach (string genre in genres)
                {
                    XmlElement newgenre = newgame_doc.CreateElement("genre");
                    XmlAttribute newgenre_att = newgame_doc.CreateAttribute("name");
                    newgenre_att.InnerText = genre;
                    newgenre.Attributes.Append(newgenre_att);
                    genres_toadd.AppendChild(newgenre);
                }
            }
            game.AppendChild(genres_toadd);

            XmlElement romhashes_toadd = newgame_doc.CreateElement("romhashes");
            game.AppendChild(romhashes_toadd);

            XmlAttribute title_toadd = newgame_doc.CreateAttribute("title");
            XmlAttribute url_toadd = newgame_doc.CreateAttribute("url");
            XmlAttribute console_toadd = newgame_doc.CreateAttribute("console");
            XmlAttribute xmlns = newgame_doc.CreateAttribute("xmlns");

            title_toadd.InnerText = title;
            url_toadd.InnerText = url;
            console_toadd.InnerText = console;
            xmlns.InnerText = "http://ect2016/GamesSchema";

            game.Attributes.Append(xmlns);
            game.Attributes.Append(title_toadd);
            game.Attributes.Append(url_toadd);
            game.Attributes.Append(console_toadd);

            newgame_doc.AppendChild(game);

            System.Data.SqlClient.SqlConnection sqlCon =
            new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "insert into dbo.Games(xmlContent) values(@newgame)";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "newgame",
                Value = newgame_doc.OuterXml
            });
            Cmd.Connection = sqlCon;
            sqlCon.Open();
            int result = Cmd.ExecuteNonQuery();
            sqlCon.Close();
        }

        public string fixGameTitle(string original_gametitle) // Ajustes para jogos cujo nome não se encontre correcto na Wiki
        {
            string fixed_gametitle = original_gametitle;
            if (fixed_gametitle.Contains("Pokémon FireRed")) fixed_gametitle = "Pokémon FireRed";
            if (fixed_gametitle.Contains("Pokémon LeafGreen")) fixed_gametitle = "Pokémon LeafGreen";
            return fixed_gametitle;
        }

        public List<string> fetchGenre(string gameurl, string platform) // Adquirir géneros de um jogo
        {
            List<string> unparsed_genres = new List<string>();
            List<string> genres = new List<string>();
            string genretext = "";
            HtmlDocument basepage = new HtmlAgilityPack.HtmlDocument();
            if (platform.Equals("nintendo")) basepage.LoadHtml(new WebClient().DownloadString("http://nintendo.wikia.com" + gameurl));
            else if (platform.Equals("sega")) basepage.LoadHtml(new WebClient().DownloadString("http://segaretro.org" + gameurl));
            WebClient cli = new WebClient();
            cli.Encoding = System.Text.Encoding.UTF8;

            if (platform.Equals("nintendo")) {
                HtmlNodeCollection rows = basepage.DocumentNode.SelectNodes("//table[@class='infobox']/tr");

                foreach (HtmlNode row in rows)
                {
                    HtmlNodeCollection subnodes = row.ChildNodes;
                    if (subnodes[1].InnerText.Contains("Genre(s)"))
                    {
                        genretext = subnodes[2].InnerText.Trim();
                    }

                }
            }
            else if (platform.Equals("sega"))
            {
                HtmlNodeCollection rows = basepage.DocumentNode.SelectNodes("//table[@class='breakout']/tr");

                foreach (HtmlNode row in rows)
                {
                    HtmlNodeCollection subnodes = row.ChildNodes;
                    if (subnodes[1].InnerText.Contains("Genre"))
                    {
                        genretext = subnodes[1].InnerText.Trim().Split(":".ToCharArray())[1].Trim();
                        System.Diagnostics.Debug.WriteLine(genretext);

                    }

                }
            }

            foreach (string part in genretext.Split(','))
            {
                foreach(string subpart in part.Split('/'))
                {
                    unparsed_genres.Add(subpart.Trim());
                }
            }
            foreach (string genre in unparsed_genres)       // Normalização de géneros entre consolas
            {
                if (genre.ToUpper().Contains("PLATFORMER") || genre.ToUpper().Contains("PLATFORMING") || genre.ToUpper().Contains("PLATFORM")) {
                    genres.Add("Platform");
                }
                else if (genre.ToUpper().Contains("ACTION"))
                {
                    genres.Add("Action");
                }
                else if (genre.ToUpper().Contains("ADVENTURE"))
                {
                    genres.Add("Adventure");
                }
                else if (genre.ToUpper().Contains("ROLE"))
                {
                    genres.Add("RPG");
                }
                else if (genre.ToUpper().Contains("CARD"))
                {
                    genres.Add("Cards");
                }
                else if (genre.ToUpper().Contains("RACING") || genre.ToUpper().Contains("RACER"))
                {
                    genres.Add("Racing");
                }
                else if (genre.ToUpper().Contains("MAZE"))
                {
                    genres.Add("Maze");
                }
                else if (genre.ToUpper().Contains("SIMULATION"))
                {
                    genres.Add("Simulation");
                }
                else if (genre.ToUpper().Contains("EDUTAINMENT"))
                {
                    genres.Add("Educational");
                }
                else if (genre.ToUpper().Contains("HACK"))
                {
                    genres.Add("Hack and Slash");
                }
                else if (genre.ToUpper().Contains("COMPILATION") || genre.ToUpper().Contains("VARIETY") || genre.ToUpper().Contains("VARIOUS"))
                {
                    genres.Add("Compilation");
                }
                else if (genre.ToUpper().Contains("PUZZLE"))
                {
                    genres.Add("Puzzle");
                }
                else if (genre.ToUpper().Contains("PARTY"))
                {
                    genres.Add("Party");
                }
                else if (genre.ToUpper().Equals("ACTION ADVENTURE") || genre.ToUpper().Equals("ACTION-ADVENTURE"))
                {
                   genres.Add("Action");
                   genres.Add("Adventure");
                }
                else if (genre.ToUpper().Equals("FARMING SIMULATOR"))
                {
                    genres.Add("Farming");
                    genres.Add("Simulation");
                }
                else if (genre.ToUpper().Equals("GOLF") || genre.ToUpper().Equals("TENNIS") || genre.ToUpper().Equals("VOLLEY") ||
                         genre.ToUpper().Contains("FOOTBALL") || genre.ToUpper().Contains("BOXING") || genre.ToUpper().Equals("SKII") ||
                         genre.ToUpper().Equals("SOCCER") || genre.ToUpper().Contains("SOCCER") || genre.ToUpper().Contains("SPORT") || genre.ToUpper().Contains("HOCKEY"))
                {
                    if(!genres.Contains("Sports")) genres.Add("Sports");
                }
                else if (genre.ToUpper().Contains("1ST-PERSON SHOOTER") || genre.ToUpper().Contains("FIRST-PERSON SHOOTER"))
                {
                    genres.Add("First-person shooter");
                }
                else if (genre.ToUpper().Contains("THIRD-PERSON SHOOTER"))
                {
                    genres.Add("Third-person shooter");
                }
                else if (genre.ToUpper().Contains("SHOOT"))
                {
                    genres.Add("Shoot 'em up");
                }
                else if (genre.ToUpper().Contains("SIDE SCROLL") || genre.ToUpper().Contains("SIDESCROLL"))
                {
                    genres.Add("Side-scroller");
                }
                else if (genre.ToUpper().Contains("METROIDVANIA"))
                {
                    genres.Add("Action");
                    genres.Add("Adventure");
                }
                else if (genre.ToUpper().Contains("LIGHT GUN"))
                {
                    genres.Add("Light gun");
                }
                else if(!genre.ToUpper().Equals("GOD") && !genre.ToUpper().Contains("RUN &"))
                {
                    System.Diagnostics.Debug.WriteLine(genre);
                    if (genre.Length > 1) genres.Add(genre.ToUpper()[0] + genre.Substring(1));
                    else genres.Add(genre);
                }
            }
            return genres;
        }
    }




}