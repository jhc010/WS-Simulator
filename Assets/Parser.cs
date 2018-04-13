using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;

public class Parser : MonoBehaviour
{
    public static String[] s = new String[50];
    public static NetworkManager net;
    Player player = null;

    public void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }
    }

    public void parse()
    {
        int counter = 0;
        Text t = this.GetComponent<Text>();
        String toParse = t.text;

        String[] lines = toParse.Split('\n');
        int cardCount = 0;
        for (int i = 0; i < lines.Length - 2; i++)
        {
            String line = lines[i];
            if (line.Split('/').Length > 1 && line[0] != '[')
            {
                String[] splitSlash = line.Split('/');
                String[] splitDash = splitSlash[1].Split('-');
                String series = splitDash[0];
                String card;
                if (splitDash[1][0] == 'T' || splitDash[1][0] == 't')
                {
                    card = splitDash[1].Substring(0, 3);
                    card = card.ToLower();
                }
                else if (splitDash[1][0] == 'P' || splitDash[1][0] == 'p')
                {
                    card = splitDash[1].Substring(0, 3);
                }
                else if (series[1] == 'E' || series[1] == 'e')
                {
                    card = splitDash[1].Substring(0, 2);
                }
                else
                {
                    card = splitDash[1].Substring(0, 3);
                }
                char count = line[line.Length - 2];
                
                String s = "";
                for (int j = 0; j < Char.GetNumericValue(count); j++)
                {
                    s = series + "/" + card;
                    Parser.s[counter] = s;
                    counter++;
                    cardCount++;
                    Debug.Log(s);
                }
                if (cardCount == 50)
                {
                    break;
                }
            }
        }
        if (NetworkServer.active)
        {
            for (int i = 0; i < 50; i++)
            {
                player.Cmd_Start_Host(s[i], player.netId);
            }
        }
        else
        {
            GameObject board = GameObject.Find("Board");
            board.transform.Rotate(new Vector3(0, 0, 180));
            for (int i = 0; i < 50; i++)
            {
                player.Cmd_Start_Client(s[i], player.netId);
            }
        }
        GameObject input = GameObject.Find("Input");
        input.SetActive(false);

    }
}
