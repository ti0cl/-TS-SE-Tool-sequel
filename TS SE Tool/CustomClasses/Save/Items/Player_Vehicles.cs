using System;
using System.Collections.Generic;
using System.Text;
using TS_SE_Tool.Save.DataFormat;

namespace TS_SE_Tool.Save.Items
{
    class Player_Vehicles : SiiNBlockCore
    {
        internal string vehicle { get; set; } = "";
        internal SCS_Placement stored_vehicle_placement { get; set; } = new SCS_Placement();
        internal string trailer { get; set; } = "";
        internal int stored_trailer_placements { get; set; } = 0;
        internal bool stored_trailer_attached { get; set; } = false;

        internal Player_Vehicles()
        { }

        internal Player_Vehicles(string[] input)
        {
            foreach (string currentLine in input)
            {
                string tagLine = "";
                string dataLine = "";

                if (currentLine.Contains(":"))
                {
                    string[] splitLine = currentLine.Split(new char[] { ':' }, 2, StringSplitOptions.None);
                    tagLine = splitLine[0].Trim();
                    dataLine = splitLine[1].Trim();
                }
                else
                {
                    tagLine = currentLine.Trim();
                }

                try
                {
                    switch (tagLine)
                    {
                        case "":
                        case "player_vehicles":
                        case "}":
                            break;
                        case "vehicle":
                            vehicle = dataLine;
                            break;
                        case "stored_vehicle_placement":
                            stored_vehicle_placement = new SCS_Placement(dataLine);
                            break;
                        case "trailer":
                            trailer = dataLine;
                            break;
                        case "stored_trailer_placements":
                            stored_trailer_placements = int.Parse(dataLine);
                            break;
                        case "stored_trailer_attached":
                            stored_trailer_attached = bool.Parse(dataLine);
                            break;
                        default:
                            UnidentifiedLines.Add(currentLine);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Utilities.IO_Utilities.ErrorLogWriter(WriteErrorMsg(ex.Message, tagLine, dataLine));
                    UnidentifiedLines.Add(currentLine);
                }
            }
        }

        internal string PrintOut(uint version, string nameless)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("player_vehicles : " + nameless + " {");
            result.AppendLine(" vehicle: " + vehicle);
            result.AppendLine(" stored_vehicle_placement: " + stored_vehicle_placement);
            result.AppendLine(" trailer: " + trailer);
            result.AppendLine(" stored_trailer_placements: " + stored_trailer_placements);
            result.AppendLine(" stored_trailer_attached: " + stored_trailer_attached.ToString().ToLower());

            if (UnidentifiedLines.Count > 0)
                result.AppendLine(WriteUnidentifiedLines());

            result.AppendLine("}");
            removeWritenBlock(nameless);
            return result.ToString();
        }
    }
}
