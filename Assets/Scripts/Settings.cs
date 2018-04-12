using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

    /// <summary>
    /// Configuration data used for parding the PDF.
    /// Defines whitespaces, headers, stopwords, etc.
    /// </summary>
    class Settings
    {
        public string UnusualCharacters { get; private set; }
        public string Header { get; private set; }
        public string Input { get; private set; }
        public string Output { get; private set; }
        public string Scoring { get; private set; }
        public string Sample { get; private set; }

        public Settings()
        {
            UnusualCharacters = Char.ConvertFromUtf32(0x200B);
            Header = @"^H\s*O\s*N\s*I[\w\W]*/\s*\d+\s*b\s*o\s*d\s*o\s*v\s*a";
            Input = @"U\s*L\s*A\s*Z\s*N\s*I\s*P\s*O\s*D\s*A\s*C\s*I";
            Output = @"I\s*Z\s*L\s*A\s*Z\s*N\s*I\s*P\s*O\s*D\s*A\s*C\s*I";
            Scoring = @"B\s*O\s*D\s*O\s*V\s*A\s*N\s*J\s*E";
            Sample = @"P\s*R\s*I\s*M\s*J\s*E\s*R\s*I\s*T\s*E\s*S\s*T\s*P\s*O\s*D\s*A\s*T\s*A\s*K\s*A";
        }
    }
