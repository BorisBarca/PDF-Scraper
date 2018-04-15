using UnityEngine;
using System;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using UnityEngine.UI;
using TMPro;
using SFB;

public class Reader : MonoBehaviour
{
    public TMP_InputField task, file, first, last, sample, ans, year, razina;
    public TMP_Dropdown vrsta;
    public GameObject panel;
    int honi;

    public void Main()
    {
        string filePath = file.text;
        int startPage = Int32.Parse(first.text);
        int endPage = Int32.Parse(last.text);
        var level = razina.text;
        int godina = Int32.Parse(year.text);
        string samplePath = sample.text;
        string name = task.text;
        string dummy_in = ".dummy.in.";
        string dummy_out = ".dummy.out.";
        Settings settings = new Settings();
        if (level[0] == 'Z')
            level = "ŽUP";
        if (level[0] == 'S')
            level = "ŠKO";

        String text = "";
        using (PdfReader reader = new PdfReader(filePath))
        {
            for (int i = startPage; i <= endPage; i++)
            {
                var page = PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy());
                page = Regex.Replace(page, settings.UnusualCharacters, "").Trim();
                Debug.Log(page);
                if (honi == 0){
                    page = Regex.Replace(page, settings.Header, "").Trim();
                } else{
                    if (godina == 2018){
                        //za dodati
                    } else if (godina == 2017 || godina == 2016){
                        int pos = 0;
                        for (; pos < page.Length; ++pos)
                            if (page[pos] == level[0])
                                break;
                        for (; pos < page.Length; ++pos){
                            if (page.Substring(pos, 6) == "bodova"){
                                pos += 6;
                                break;
                            }
                        }
                        page = page.Substring(pos, page.Length - pos);
                    } else {
                        int pos = 0;
                        for (; pos < page.Length; ++pos)
                        {
                            Debug.Log(page[pos] + " " + level[0]);
                            if (page[pos] == level[0])
                                break;
                        }
                        Debug.Log(pos);
                        for (; pos < page.Length; ++pos)
                        {
                            bool ok = true;
                            for (int k = 0; k < name.Length; ++k)
                            {
                                if (page[k + pos] != name[k])
                                    ok = false;
                            }
                            if (ok)
                            {
                                pos += name.Length;
                                break;
                            }
                        }
                        page = page.Substring(pos + 9, page.Length - pos - 9);
                    }
                }
                Debug.Log(page);
                text = text + " " + page;
            }

            var inputHeader = Regex.Match(text, settings.Input);
            var outputHeader = Regex.Match(text, settings.Output);
            var scoringHeader = Regex.Match(text, settings.Scoring);
            var sampleHeader = Regex.Match(text, settings.Sample);

            bool hasScoring = scoringHeader.Success;

            string body = text.Substring(0, inputHeader.Index);
            int start = inputHeader.Index + inputHeader.Length;
            string input = text.Substring(start, outputHeader.Index - start);
            string output;
            string scoring = "";

            if (hasScoring)
            {
                start = outputHeader.Index + outputHeader.Length;
                output = text.Substring(start, scoringHeader.Index - start);
                start = scoringHeader.Index + scoringHeader.Length;
                scoring = text.Substring(start, sampleHeader.Index - start);
            }
            else
            {
                start = outputHeader.Index + outputHeader.Length;
                output = text.Substring(start, sampleHeader.Index - start);
            }

            start = sampleHeader.Index + sampleHeader.Length;
            string samples = text.Substring(start, text.Length - start);

            body = CreateMarkdown(body);
            input = CreateMarkdown(input);
            output = CreateMarkdown(output);
            scoring = CreateMarkdown(scoring);

            int cnt = Regex.Matches(samples, "ulaz").Count;

            string primjeri = "";

            for (int i = 1; i <= cnt; ++i)
            {
                string tmp = name + dummy_in + i;
                string[] primjer = System.IO.File.ReadAllLines(samplePath + "\\" + tmp);
                primjeri += "###Ulaz\n";
                primjeri += "```\n";
                foreach (string s in primjer)
                {
                    primjeri += s + "\n";
                }
                primjeri += "```\n";
                tmp = name + dummy_out + i;
                primjer = System.IO.File.ReadAllLines(samplePath + "\\" + tmp);
                primjeri += "###Izlaz\n";
                primjeri += "```\n";
                foreach (string s in primjer)
                {
                    primjeri += s + "\n";
                }
                primjeri += "```\n";
            }

            string sol = "";


            sol = body + "\n" + "##Ulazni​ podaci" + "\n" + input + "\n" + "##Izlazni podaci" + "\n" + output;
            if (hasScoring)
                sol += "\n" + "##Bodovanje" +"\n" + scoring;
            sol += "\n" +"##Primjeri test​ podataka" + "\n" + primjeri;

            ans.text = sol;
        }
    }

    private static string CreateMarkdown(string text)
    {
        string unused = Char.ConvertFromUtf32(0x200B);
        text = Regex.Replace(text, @"\n *\n", unused);
        text = Regex.Replace(text, @"\n", "");
        text = Regex.Replace(text, unused, "\n\n");

        text = Regex.Replace(text, @" +", " ");

        text = Regex.Replace(text, @"\u25CF", "\n- ");

        text = Regex.Replace(text, @"\(", @"~(~");
        text = Regex.Replace(text, @"\)", @"~)~");
        text = Regex.Replace(text, @"\sx\s", @" ~\times~ ");
        text = Regex.Replace(text, "\u2260", @"~\neq~");
        text = Regex.Replace(text, "\u2260", @"~\neq~");
        text = Regex.Replace(text, "\u2264", @"~\leq~");
        text = Regex.Replace(text, "\u2265", @"~\geq~");
        text = Regex.Replace(text, "%", @"~\%~");

        var numbers = Regex.Matches(text, @"\d+");
        StringBuilder sb = new StringBuilder();
        int oldIndex = 0;
        foreach (Match number in numbers)
        {
            sb.Append(text.Substring(oldIndex, number.Index - oldIndex));
            sb.Append("~");
            sb.Append(text.Substring(number.Index, number.Length));
            sb.Append("~");

            oldIndex = number.Index + number.Length;
        }
        sb.Append(text.Substring(oldIndex, text.Length - oldIndex));
        text = sb.ToString();

        var variables = Regex.Matches(text, @"\s[A-HJ-TV-Z]\s");
        sb = new StringBuilder();
        oldIndex = 0;
        foreach (Match variable in variables)
        {
            sb.Append(text.Substring(oldIndex, variable.Index - oldIndex));
            sb.Append(" ~");
            sb.Append(text.Substring(variable.Index + 1, variable.Length - 2));
            sb.Append("~ ");

            oldIndex = variable.Index + variable.Length;
        }
        sb.Append(text.Substring(oldIndex, text.Length - oldIndex));
        text = sb.ToString();

        text = Regex.Replace(text, @"~\s*,\s*~", ", ");
        text = Regex.Replace(text, @"~\s*~", " ");

        text = Regex.Replace(text, @"~\)~", @")");
        text = Regex.Replace(text, @"~\(~", @"(");

        return text;
    }

    public void openPDF()
    {
        var put = StandaloneFileBrowser.OpenFilePanel("Open pdf", "", "pdf", false);
        if (put.Length >= 1) {
            string path = put[0];

            file.text = path;
        }
    }

    public void openSample()
    {
        var put = StandaloneFileBrowser.OpenFolderPanel("Open folder", "", false);
        if (put.Length >= 1)
        {
            string path = put[0];

            sample.text = path;
        }
    }

    void Update()
    {
        honi = vrsta.value;
        if (honi == 1)
            panel.SetActive(false);
        else
            panel.SetActive(true);
    }
}
