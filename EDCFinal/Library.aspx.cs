using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Microsoft.AspNet.Identity;


namespace EDCFinal
{
    public partial class Library : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Context.User.Identity.GetUserId() == "190d22ec-44a8-4bf4-9579-0365a6b5af3e")
            {
                library_title.InnerHtml = "<i class=\"fa fa-gamepad\"></i>&nbsp;&nbsp;Managing Public Library</h3>";
            }

            string qs = "";
            try {
                qs = Request.QueryString["d"];
                deleteROM(qs);
            }
            catch (Exception)
            {
                
            }
            XmlDocument games = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno Humberto\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) SELECT UserGames.xmlContent.query('( for $game in /us:user/us:games/us:game return $game)') AS game FROM UserGames WHERE UserGames.xmlContent.exist('/us:user[@id=sql:variable(\"@userid\")]') = 1;";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            var gamereader = Cmd.ExecuteReader();

            var gameindex = gamereader.GetOrdinal("game");
            while (gamereader.Read())
            {
                games.LoadXml("<games>" + (string) gamereader.GetValue(gameindex) + "</games>");
            }
            sqlCon.Close();

            foreach (XmlNode game_list in games.ChildNodes)
            {
                foreach (XmlNode game in game_list.ChildNodes)
                {
                    string console = "";
                    switch (game.Attributes["console"].Value)
                    {
                        case "GBC":
                            console = "GameBoy Color";
                            break;
                        case "GB":
                            console = "Classic GameBoy";
                            break;
                        case "NES":
                            console = "Nintendo Entertainment System&nbsp;&nbsp;&nbsp;";
                            break;
                        case "GBA":
                            console = "GameBoy Advance&nbsp;&nbsp;&nbsp;";
                            break;
                        case "SMS":
                            console = "Sega Master System";
                            break;
                        default:
                            break;
                    }
                    gamecontainer.InnerHtml += "<div class=\"col-md-12\" style=\"margin: 3px; border-style: solid; border-color: #596a7b;\">" +
                          "<div vocab=\"http://schema.org/\" typeof=\"VideoGame\" class=\"row\" style=\"padding: 10px\">" +
                          "<div class=\"col-md-4\" style=\"text-align: center\">" +
                          "<img style=\"max-height: 300px;\"property=\"image\" src=\"" + game.Attributes["imgurl"].Value + "\" />" +
                          "</div>" +
                          "<div class=\"col-md-5\" style=\"padding: 30px\">" +
                          "<h4 property=\"name\">" + game.Attributes["title"].Value + "</h3>" +
                          "<h5 property=\"datePublished\">Release Date:&nbsp;&nbsp;<small>" + game.Attributes["releasedate"].Value + "</small></h4>" +
                          "<h5 property=\"genre\">Genre:&nbsp;&nbsp;<small>" + game.Attributes["genres"].Value + "</small></h4>" +
                          "<h5 property=\"author\">Developer:&nbsp;&nbsp;<small>" + game.Attributes["developers"].Value + "</small></h4>" +
                          "<h5 property=\"publisher\">Publisher:&nbsp;&nbsp;<small>" + game.Attributes["publishers"].Value + "</small></h4>" +
                          "<h5 property=\"gamePlatform\">Platform:&nbsp;&nbsp;<small>" + console + "<img src=\"/Content/" + game.Attributes["console"].Value + ".png\" style=\"max-height:32px\" /></small></h4>" +
                          "</div>" +
                          "<div class=\"col-md-3\" style=\"text-align: center\">" +
                          "<a style=\"margin-top: 100px; width: 165px\" href=\"play" + game.Attributes["console"].Value + "?h=" + game.Attributes["filename"].Value + "\" class=\"btn btn-primary\" role=\"button\">Play now!</a><br>" +
                          "<a style=\"margin-top: 5px; margin-right: 5px; width: 80px;\" href=\"Library" + "?d=" + game.Attributes["filename"].Value + "\" class=\"btn btn-danger\" role=\"button\">Delete</a>" +
                          "<a style=\"margin-top: 5px; width: 80px;\" href =\"editROM" + "?h=" + game.Attributes["filename"].Value + "\" class=\"btn btn-success\" role=\"button\">Edit</a>" +
                          "</div>" +
                          "</div>" +
                          "</div>";

                }

            }
        }
        public void deleteROM(string filename)
        {
            bool gameexists;
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) SELECT UserGames.id FROM UserGames WHERE UserGames.xmlContent.exist('//*[@id=sql:variable(\"@userid\")]//*[@filename=sql:variable(\"@hash\")] ') = 1; ";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "hash",
                Value = filename
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            var gamereader = Cmd.ExecuteReader();
            if (!gamereader.HasRows) gameexists = false;
            else gameexists = true;
            sqlCon.Close();
            if (!gameexists) System.Diagnostics.Debug.WriteLine("O jogo não existe neste utilizador");
            else System.Diagnostics.Debug.WriteLine("O jogo já existe neste utilizador");

            if (gameexists)
            {
                sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
                Cmd = new System.Data.SqlClient.SqlCommand();
                Cmd.CommandText = "with xmlnamespaces('http://ect2016/UserGamesSchema' as us) update UserGames set xmlContent.modify('delete /us:user/us:games/us:game[@filename=sql:variable(\"@hash1\")]') where xmlContent.exist('/us:user[@id=sql:variable(\"@userid\")]//*[@filename=sql:variable(\"@hash1\")]') = 1";
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "userid",
                    Value = User.Identity.GetUserId()
                });
                Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "hash1",
                    Value = filename
                });

                Cmd.Connection = sqlCon;
                sqlCon.Open();
                int result = Cmd.ExecuteNonQuery();
                sqlCon.Close();
            }
        }
    }
}