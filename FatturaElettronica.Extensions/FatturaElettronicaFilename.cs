﻿using FatturaElettronica.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FatturaElettronica.Extensions
{
    /// <summary>
    /// Factory per la creazione del filename
    /// </summary>
    public class FatturaElettronicaFilename
    {
        private IdFiscaleIVA _idFiscaleIVA;
        private FatturaExtensionType _fatturaExtensionType;
        public FatturaElettronicaFilename(IdFiscaleIVA idFiscale, FatturaExtensionType fatturaType = FatturaExtensionType.Plain)
        {
            _idFiscaleIVA = idFiscale ?? throw new ArgumentNullException("Id Fiscale non specificato");
            if (_idFiscaleIVA.IdPaese == null || _idFiscaleIVA.IdPaese.Length < 2)
                throw new ArgumentException("IdPaese non specificato");
            if (_idFiscaleIVA.IdCodice == null)
                throw new ArgumentException("IdCodice non specificato");
            _fatturaExtensionType = fatturaType;
        }
        /// <summary>
        /// Restituisce il nome del file partendo dall'ultimo numero di fattura staccata
        /// </summary>
        /// <param name="lastBillingNumber"></param>
        /// <returns></returns>
        public string FileName(int lastBillingNumber) => $"{_idFiscaleIVA.IdPaese}{_idFiscaleIVA.IdCodice}_{GetNextFilename(lastBillingNumber)}{Extension}";
        /// <summary>
        /// Restituisce il nome del file partendo dall'ultimo nome del file generato
        /// </summary>
        /// <param name="lastBillingNumber"></param>
        /// <returns></returns>
        public string FileName(string lastBillingNumber) => $"{_idFiscaleIVA.IdPaese}{_idFiscaleIVA.IdCodice}_{GetNextFilename(lastBillingNumber)}{Extension}";

        private char[] baseChars = new char[] { '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

        public int? CurrentIndex { get; set; } = null;

        private string Extension => _fatturaExtensionType == FatturaExtensionType.Plain ? ".xml" : ".xml.p7m";

        /// <summary>
        /// Restituisce il Progressivo Invio Successivo
        /// </summary>
        /// <param name="lastNumber"></param>
        /// <returns></returns>
        private string GetNextFilename(int lastNumber)
        {
            CurrentIndex = ++lastNumber;
            string value = ToString(CurrentIndex.Value);
            if (value.Length > 5)
                throw new OverflowException("Il progressivo è troppo elevato");
            return value.PadLeft(5, '0');
        }

        /// <summary>
        /// Restituisce il Progressivo Invio Successivo
        /// </summary>
        /// <param name="lastStringNumber"></param>
        /// <returns></returns>
        private string GetNextFilename(string lastStringNumber)
        {
            return GetNextFilename(ToInteger(lastStringNumber));
        }

        /// <summary>
        /// Da numero a stringa valida
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ToString(int value)
        {
            int i = 32;
            char[] buffer = new char[i];
            int targetBase = baseChars.Length;

            do
            {
                buffer[--i] = baseChars[value % targetBase];
                value = value / targetBase;
            }
            while (value > 0);

            char[] result = new char[32 - i];
            Array.Copy(buffer, i, result, 0, 32 - i);

            return new string(result);
        }

        /// <summary>
        /// Da Stringa valida a numero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int ToInteger(string value)
        {
            int i = 32, k = value.Length - 1;
            value = Reverse(value);
            char[] buffer = new char[i];
            int targetBase = baseChars.Length;
            int result = 0;

            do
            {
                for (int j = baseChars.Length - 1; j > 0; j--)
                {
                    if (baseChars[j] == value[k])
                    {
                        result = result + (j * ((int)Math.Pow(targetBase, k)));
                        break;
                    }
                }
                k--;
            }
            while (k >= 0);
            return result;
        }

        /// <summary>
        /// String Reverse
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
