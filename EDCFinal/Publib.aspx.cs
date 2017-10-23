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
    public partial class Publib : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            if (Context.User.Identity.GetUserId() == "190d22ec-44a8-4bf4-9579-0365a6b5af3e") Response.Redirect("/Library");

            XmlDocument games = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno Humberto\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) SELECT UserGames.xmlContent.query('( for $game in /us:user/us:games/us:game return $game)') AS game FROM UserGames WHERE UserGames.xmlContent.exist('/us:user[@id=sql:variable(\"@userid\")]') = 1;";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = "190d22ec-44a8-4bf4-9579-0365a6b5af3e"
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            var gamereader = Cmd.ExecuteReader();

            var gameindex = gamereader.GetOrdinal("game");
            while (gamereader.Read())
            {
                games.LoadXml("<games>" + (string)gamereader.GetValue(gameindex) + "</games>");
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
                          "<a style=\"margin-top: 100px\" href=\"play" + game.Attributes["console"].Value + "?h=" + game.Attributes["filename"].Value + "\" class=\"btn btn-primary\" role=\"button\">Play now!</a>" +
                          "</div>" +
                          "</div>" +
                          "</div>";

                }

            }
        }
    }
}