using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;

namespace EDCFinal
{
    public partial class Upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void upload_button_Click(object sender, EventArgs e)
        {
            if (upload_element.HasFile)
            {
                    string chosen_name = Path.GetFileName(upload_element.FileName);
                    string extension = "";
                    bool found = false;
                    bool isGB = false;
                    string nameonly = "";
                    string ext = "";

                    if (chosen_name.Length > 3)
                    {
                        extension = chosen_name.Substring(chosen_name.Length - 4).ToUpper();
                        if (extension != ".NES" && extension != ".GBC" && extension != ".GBA" && extension != ".SMS" && (!extension.EndsWith(".GB")))
                        {
                            unsuccessful(null);
                            return;
                        }
                        else
                        {
                            if (extension.EndsWith(".GB")) isGB = true;
                        }
                    }
                    else
                    {
                        unsuccessful(null);
                        return;
                    }
                    upload_element.SaveAs(Server.MapPath("~/ROMS/") + chosen_name);

                    if(!isGB)
                    {
                        nameonly = chosen_name.Remove(chosen_name.Length - 4);
                        ext = extension.Substring(extension.Length - 3);
                    }
                    else
                    {
                        nameonly = chosen_name.Remove(chosen_name.Length - 3);
                        ext = "GB";
                        extension = ".GB";
                    }

                    nameonly = (Regex.Replace(nameonly, @"\s*?(?:\(.*?\)|\[.*?\]|\{.*?\})", String.Empty)).Trim();  // Remover todo o conteúdo entre (), [] ou {}

                    string hashstring = "";

                    using (FileStream stream = File.OpenRead(Server.MapPath("~/ROMS/") + chosen_name))
                    {
                        SHA1Managed sha = new SHA1Managed();
                        byte[] hash = sha.ComputeHash(stream);
                        hashstring = BitConverter.ToString(hash).Replace("-", String.Empty);
                    }


                    if (findCandidates(nameonly, ext, hashstring, true).Count != 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Existe um rom com este hash!" + nameonly + " " + ext);
                        found = true;
                    }
                    else System.Diagnostics.Debug.WriteLine("Não existe um rom com este hash!" + nameonly + " " + ext);

                    FileInfo saved_rom = new FileInfo(Server.MapPath("~/ROMS/") + chosen_name);
                    if (saved_rom.Exists)
                    {
                        if (found) saved_rom.Delete();
                        else
                        {
                            FileInfo hashed_check = new FileInfo(Server.MapPath("~/ROMS/") + hashstring + extension);
                            if (!hashed_check.Exists) saved_rom.MoveTo(Server.MapPath("~/ROMS/") + hashstring + extension);
                        }
                    
                    }


                    uploadSucceeded(nameonly, ext, hashstring, found);
            }
        }

        public void uploadSucceeded(string filename, string console, string hash, bool found)
        {
            List<Tuple<string, string, string, string>> candidates = new List<Tuple<string, string, string, string>>();
            Tuple<string, string, string, string> chosen;
            Boolean ok = false;
            if (found)
            {
                chosen = findCandidates(filename, console, hash, true)[0];
                ok = true;
            }
            else
            {
                candidates = findCandidates(filename, console, "", false);
                if (candidates.Count == 1)
                {
                    chosen = candidates[0];
                    System.Diagnostics.Debug.WriteLine(chosen.Item1);
                    addHashToGame(chosen, hash);
                    ok = true;
                }
                else if (candidates.Count > 1)
                {
                    chosen = candidates[0];
                    System.Diagnostics.Debug.WriteLine(chosen.Item1);
                    addHashToGame(chosen, hash);
                    ok = true;
                }
                else
                {
                    chosen = null;
                }
            }

            if (ok) presentGame(chosen, hash);
            else unsuccessful(hash + "." + console);
            
        }

        public void unsuccessful(string filename)
        {
            gamecontainer.InnerHtml = "<div class=\"col-md-6 col-md-offset-3\" style=\"border-style: solid; border-color: #596a7b;\">" +
                                      "<div class=\"row\" style=\"padding: 10px\">" +
                                      "<div class=\"col-md-12\" style=\"padding-left: 30px; padding-right: 30px; padding-top: 10px; padding-bottom: 30px\">" +
                                      "<h3>" + "Unable to recognize your ROM" + "</h3>" +
                                      "<h4>Possible reasons:</h4><div style=\"padding-left: 40px\">" +
                                      "<h5>Wrong extension.&nbsp;&nbsp;<small>Only GB, GBC, GBA and NES are supported.</small></h5>" +
                                      "<h5>No ROM found with that filename</h5>" +
                                      "<h5>No ROM found that contains that filename</h5>" +
                                      "</div></div>" +
                                      "</div>" +
                                      "</div>";
           if (filename != null)
            {
                FileInfo saved_file = new FileInfo(Server.MapPath("~/ROMS/") + filename);
                if (saved_file.Exists)
                {
                    saved_file.Delete();
                }
            }
        }

        public void addHashToGame(Tuple<string, string, string, string> candidate, string hash) // Adicionar o hash do ROM à lista de hashes esperados para este jogo
        {
            System.Diagnostics.Debug.WriteLine("Adding hash " + hash + "(" + candidate.Item1 + ") - " + candidate.Item4 + " to DB.");
            System.Data.SqlClient.SqlConnection sqlCon =
                    new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/GamesSchema' as gm) UPDATE Games SET xmlContent.modify('insert <gm:romhash value=\"{ sql:variable(\"@hasharg\") }\" /> as first into (/gm:game[@title=sql:variable(\"@gametitle\") and @console=sql:variable(\"@gameconsole\")]/gm:romhashes) ')";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "hasharg",
                Value = hash
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "gametitle",
                Value = candidate.Item1
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "gameconsole",
                Value = candidate.Item4
            });
            Cmd.Connection = sqlCon;
            sqlCon.Open();
            int result = Cmd.ExecuteNonQuery();
            sqlCon.Close();
        }

        public string prettyPrint(string listholder)
        {
            listholder = listholder.Trim();
            string result = "";
            if (listholder.Equals("unavailable")) return "unavailable";

            if (listholder.Length > 0)
            {
                result = (listholder.Replace(",", ", ")).Trim();
                if (result.EndsWith(",")) result = result.Remove(result.Length - 1);
            }

            return result;
        }

        public bool addToUserLibrary(string developers, string publishers, string genres, string title, string releasedate, string image, string hash, string console, bool ispublic) //Adicionar jogo à biblioteca do utilizador
        {
            bool userexists;
            System.Data.SqlClient.SqlConnection sqlCon =
            new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) SELECT UserGames.id FROM UserGames WHERE UserGames.xmlContent.exist('//*[string(@id)=sql:variable(\"@userid\")] ') = 1; ";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Connection = sqlCon;
            sqlCon.Open();
            var gamereader = Cmd.ExecuteReader();
            if (!gamereader.HasRows) userexists = false;
            else userexists = true;
            sqlCon.Close();
            if (!userexists) System.Diagnostics.Debug.WriteLine("User não existe na DB");
            else System.Diagnostics.Debug.WriteLine("User existe");

            if (!userexists) // Utilizador nunca teve nenhum jogo associado? Então é necessário inicializar o utilizador na tabela UserGames
            {
                XmlDocument newuser = new System.Xml.XmlDocument();
                XmlElement user = newuser.CreateElement("user");
                XmlElement games = newuser.CreateElement("games");
                user.AppendChild(games);
                XmlAttribute id = newuser.CreateAttribute("id");
                XmlAttribute xmlns = newuser.CreateAttribute("xmlns");
                xmlns.InnerText = "http://ect2016/UserGamesSchema";
                id.InnerText = User.Identity.GetUserId();
                user.Attributes.Append(id);
                user.Attributes.Append(xmlns);
                newuser.AppendChild(user);


                sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
                Cmd = new System.Data.SqlClient.SqlCommand();
                Cmd.CommandText = "insert into dbo.UserGames(xmlContent) values(@newuser)";
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "newuser",
                    Value = newuser.OuterXml
                });
                Cmd.Connection = sqlCon;
                sqlCon.Open();
                int result = Cmd.ExecuteNonQuery();
                sqlCon.Close();
            }

            bool gameexists; // O utilizador já tem este jogo na sua biblioteca?
            sqlCon = 
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) SELECT UserGames.id FROM UserGames WHERE UserGames.xmlContent.exist('//*[@id=sql:variable(\"@userid\")]//*[@filename=sql:variable(\"@hash\")] ') = 1; ";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "hash",
                Value = hash
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            gamereader = Cmd.ExecuteReader();
            if (!gamereader.HasRows) gameexists = false;
            else gameexists = true;
            sqlCon.Close();
            if (!gameexists) System.Diagnostics.Debug.WriteLine("O jogo não existe neste utilizador");
            else {
                System.Diagnostics.Debug.WriteLine("O jogo já existe neste utilizador");
            }


            if (!gameexists) // Se não tem, então é necessário adicionar.
            {

                sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
                Cmd = new System.Data.SqlClient.SqlCommand();
                Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) UPDATE UserGames SET xmlContent.modify('insert <us:game developers=\"{ sql:variable(\"@devel\") }\" publishers=\"{ sql:variable(\"@publi\") }\" genres=\"{ sql:variable(\"@grs\") }\" title=\"{ sql:variable(\"@gametitle\") }\" releasedate=\"{ sql:variable(\"@release\") }\" imgurl=\"{ sql:variable(\"@iurl\") }\" filename=\"{ sql:variable(\"@hash\") }\" console=\"{ sql:variable(\"@consolename\") }\" public=\"{ sql:variable(\"@ispublic\") }\" /> as first into (/us:user[@id=sql:variable(\"@userid\")]/us:games) ')";
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "devel",
                    Value = developers
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "publi",
                    Value = publishers
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "grs",
                    Value = genres
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "gametitle",
                    Value = title
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "release",
                    Value = releasedate
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "iurl",
                    Value = image
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "hash",
                    Value = hash
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "consolename",
                    Value = console
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "ispublic",
                    Value = ispublic
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "userid",
                    Value = User.Identity.GetUserId()
                });
                Cmd.Connection = sqlCon;
                sqlCon.Open();
                int result = Cmd.ExecuteNonQuery();
                sqlCon.Close();
                return true;
            }
            else return false;


        }


        public void presentGame(Tuple<string, string, string, string> candidate, string hash) // Coloca os dados obtidos sobre o jogo na página web.
        {
            Tuple<string, string, string, string> gameData = fixExceptions(scrapGameData(candidate.Item2, candidate.Item4), candidate.Item1, candidate.Item2);
            string title = candidate.Item1;
            string genres = prettyPrint(candidate.Item3);
            string console = candidate.Item4;
            string image = gameData.Item1;
            string releasedate = gameData.Item2;
            string developers = prettyPrint(gameData.Item3);
            string publishers = prettyPrint(gameData.Item4);

            bool result = addToUserLibrary(developers, publishers, genres, title, releasedate, image, hash, console, false);

            string resultstring = "";
            if (result)
            {
                if (Context.User.Identity.GetUserId() == "190d22ec-44a8-4bf4-9579-0365a6b5af3e") resultstring = "<h5><span class=\"bg-success\">Added to the public library!</span></h5>";
                else resultstring = "<h5><span class=\"bg-success\">Added to your private library!</span></h5>";
            }
            else
            {
                if (Context.User.Identity.GetUserId() == "190d22ec-44a8-4bf4-9579-0365a6b5af3e") resultstring = "<h5><span class=\"bg-danger\">Already in the public library!</span></h5>";
                else resultstring = "<h5><span class=\"bg-danger\">Already in your private library!</span></h5>";
            }

            gamecontainer.InnerHtml = "<div class=\"col-md-8 col-md-offset-2\" style=\"border-style: solid; border-color: #596a7b;\">" +
                                      "<div class=\"row\" style=\"padding: 10px\">" +
                                      "<div class=\"col-md-4\" style=\"text-align: center\">" +
                                      "<img src=\"" + image + "\" />" +
                                      "</div>" +
                                      "<div class=\"col-md-8\" style=\"padding-left: 30px; padding-right: 30px; padding-top: 10px; padding-bottom: 30px\">" +
                                      "<h3>" + title + "</h3>" +
                                      "<h4>Release Date:&nbsp;&nbsp;<small>" + releasedate + "</small></h4>" +
                                      "<h4>Genre:&nbsp;&nbsp;<small>" + genres + "</small></h4>" +
                                      "<h4>Developer:&nbsp;&nbsp;<small>" + developers + "</small></h4>" +
                                      "<h4>Publisher:&nbsp;&nbsp;<small>" + publishers + "</small></h4>" +
                                      "<h4>Platform:&nbsp;&nbsp;<small>" + console + "</small></h4>" +
                                      resultstring + 
                                      "</div>" +
                                      "</div>" +
                                      "</div>";

            

        }

        public Tuple<string, string, string, string> fixExceptions(Tuple<string, string, string, string> gamedata, string gametitle, string gameurl)
        {
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

        public Tuple<string, string, string, string> scrapGameData(string url, string console) // Obter toda a informação sobre o jogo através de scraping
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

                foreach (HtmlNode row in rows)
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
                    foreach (HtmlNode descendant in subnodes.Descendants())
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
                    foreach (HtmlNode descendant in subnodes.Descendants())
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

        public List<Tuple<string, string, string, string>> findCandidates(string filename, string console, string romhash, bool found) // Obter possiveis jogos associados a este nome
        {
            List<Tuple<string, string, string, string>> results = new List<Tuple<string, string, string, string>>();
            if (!found) // Assumir que o ROM nunca foi antes introduzido no sistema
            {
                System.Data.SqlClient.SqlConnection sqlCon =
                    new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
                System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
                Cmd.CommandText = "with xmlnamespaces ('http://ect2016/GamesSchema' as gm) SELECT Games.id, name = Games.xmlContent.value('/gm:game/@title', 'nvarchar(max)'), url = Games.xmlContent.value('/gm:game/@url', 'nvarchar(max)'), Games.xmlContent.query('( for $genre in /gm:game/gm:genres/gm:genre return concat(string($genre/@name), \",\") )') AS genres FROM Games WHERE Games.xmlContent.exist('//*[contains(@title, sql:variable(\"@titlearg\")) and @console=sql:variable(\"@consolearg\")] ') = 1";
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "titlearg",
                    Value = filename
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "consolearg",
                    Value = console
                });
                Cmd.Connection = sqlCon;
                sqlCon.Open();
                var gamereader = Cmd.ExecuteReader();

                var nameindex = gamereader.GetOrdinal("name");
                var urlindex = gamereader.GetOrdinal("url");
                var genresindex = gamereader.GetOrdinal("genres");
                while (gamereader.Read())
                {
                    string name = (string)gamereader.GetValue(nameindex);
                    string url = (string)gamereader.GetValue(urlindex);
                    string genres = (string)gamereader.GetValue(genresindex);
                    results.Add(Tuple.Create(name, url, genres, console));
                }
                sqlCon.Close();
            } else // Verificar se o ROM já existiu no sistema, pesquisa por hash.
            {
                System.Data.SqlClient.SqlConnection sqlCon =
                    new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
                System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
                Cmd.CommandText = "with xmlnamespaces ('http://ect2016/GamesSchema' as gm) SELECT Games.id, name = Games.xmlContent.value('/gm:game/@title', 'nvarchar(max)'), url = Games.xmlContent.value('/gm:game/@url', 'nvarchar(max)'), Games.xmlContent.query('( for $genre in /gm:game/gm:genres/gm:genre return concat(string($genre/@name), \",\") )') AS genres FROM Games WHERE Games.xmlContent.exist('//gm:romhash[@value=sql:variable(\"@hasharg\")] ') = 1";
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "hasharg",
                    Value = romhash
                });
                Cmd.Connection = sqlCon;
                sqlCon.Open();
                var gamereader = Cmd.ExecuteReader();

                var nameindex = gamereader.GetOrdinal("name");
                var urlindex = gamereader.GetOrdinal("url");
                var genresindex = gamereader.GetOrdinal("genres");
                while (gamereader.Read())
                {
                    string name = (string)gamereader.GetValue(nameindex);
                    string url = (string)gamereader.GetValue(urlindex);
                    string genres = (string)gamereader.GetValue(genresindex);
                    results.Add(Tuple.Create(name, url, genres, console));
                }
                sqlCon.Close();
            }
            return results;
             
        }
    }
}