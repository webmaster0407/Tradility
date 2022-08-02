Attribute VB_Name = "ImportIBKR"
Option Explicit

' SKR-03

' 0 Anlage- und Kapitalkonten
' - 0525 Wertpapiere des Anlagevermögens

' 1 Finanz- und Privatkonten
' - 1220 Bank 2
' - 1349 Wertpapieranlagen im Rahmen der kurzfristigen Finanzdisposition

' 2 Abgrenzungskosten
' - 2640 Zins- und Dividendenerträge
' - 2709 Sonstige Erträge, unregelmäßig

' 4 Betriebliche Aufwendungen
' - 4875 Abschreibungen auf Wertpapiere des Umlaufvermögens

' 7 Bestände an Erzeugnissen

' 8 Erlöskonten
' - 8818 Erlöse aus Verkäufen Finanzanlagen (bei Buchverlust)
' - 8838 Erlöse aus Verkäufen Finanzanlagen (bei Buchgewinn)

' 9 Vortrags-, Kapital-, Korrektur- und statist. Konten

Public Sub ImportIBKR()

    Dim Row As Range
        
    Dim wbIB As Workbook
    
    Dim PathDir As String
    Dim FileName As String
    Dim FilePath As String
    
    PathDir = "C:\Users\SchäferDaniel\Documents\2021\12\20"
    FileName = "U7175051_U3175051_20200102_20201231_AS_Fv2_1b9da8168495a04e57c6ff944b0765c1.csv"
    
    Application.ScreenUpdating = False
    
    Set wbIB = Workbooks.Open(FileName:=PathDir & "\" & FileName, Local:=False, ReadOnly:=True)
    
    Dim ExchangeRates As Dictionary
    Set ExchangeRates = CreateObject("Scripting.Dictionary")
    ProcessExchangeRates ExchangeRates, PathDir
        
    Dim Section As String
    Dim Selector As String
    
    Dim fso As FileSystemObject
    Dim ts As TextStream
    Set fso = CreateObject("Scripting.FileSystemObject")
    Set ts = fso.CreateTextFile(PathDir & "\EXTF_Buchungsstapel_IBKR_" & Format(Now, "yyyy-mm-dd-(h-M-s)") & ".csv")
    
    ts.WriteLine GetEXTFHeader1
    ts.WriteLine GetEXTFHeader2

    For Each Row In wbIB.ActiveSheet.UsedRange.Rows
    
        Section = Row.Cells(1, 1).Value
        Selector = Row.Cells(1, 2).Value
    
        If Selector = "Header" Then
            'Debug.Print "Skip Row " & Row.Row & " (Selector = Header)"
        ElseIf Selector = "Total" Then
            'Debug.Print "Skip Row " & Row.Row & " (Selector = Total)"
        ElseIf Selector = "SubTotal" Then
            'Debug.Print "Skip Row " & Row.Row & " (Selector = SubTotal)"
        ElseIf Section = "Notes/Legal Notes" Then
            'Debug.Print "Skip Row " & Row.Row & " (Notes/Legal Notes)"
        ElseIf Section = "Codes" Then
            'Debug.Print "Skip Row " & Row.Row & " (Codes)"
        ElseIf Section = "Financial Instrument Information" Then
            'Debug.Print "Skip Row " & Row.Row & " (Financial Instrument Information)"
        ElseIf Section = "Mark-to-Market Performance Summary" Then
            'Debug.Print "Skip Row " & Row.Row & " (Mark-to-Market Performance Summary)"
        ElseIf Section = "Dividends" Then
            HandleDividends Row, ts, ExchangeRates
        ElseIf Section = "Trades" Then
            HandleTrades Row, ts, ExchangeRates
        Else
            Debug.Print "Not Handled Row " & Row.Row & " " & Row.Cells(1, 1)
        End If
    
    Next Row
    
    ts.Close
    wbIB.Close
    
    Application.ScreenUpdating = True

End Sub

Private Sub HandleDividends(ByRef Row As Range, ByRef CsvStream As TextStream, ByRef ExchangeRates As Dictionary)

    Dim csv As String

    Dim Header As String
    Dim CurrencySymbol As String
    Dim DateColumn As Date
    Dim Description As String
    Dim Amount As Currency
    Dim EuRate As Currency
    
    Header = Row.Cells(1, 2).Value
    CurrencySymbol = Row.Cells(1, 3).Value
    Amount = Row.Cells(1, 6).Value
    
    If Header <> "Data" Then
        'Debug.Print "Dividends: Skip Row " & Row.Row & " <> Data " & Header
    ElseIf InStr(CurrencySymbol, "Total") > 0 Then
        'Debug.Print "Dividends: Skip Row " & Row.Row & " = Total"
    ElseIf Amount < 0 Then
        'Debug.Print "Dividends: Skip Row " & Row.Row & " Amount < 0"
    Else
    
        DateColumn = Row.Cells(1, 4).Value
        Description = Row.Cells(1, 5).Value
        Amount = Row.Cells(1, 6).Value
        
        EuRate = GetExchangeRate(ExchangeRates, CurrencySymbol, DateColumn)
                        
        csv = FormatCSV( _
            Amount:=Round(Amount, 2), _
            CurrencySymbol:=CurrencySymbol, _
            EuRate:=Round(EuRate, 2), _
            AmountEUR:=Round(Amount / EuRate, 2), _
            Account1:="1220", _
            Account2:="2640", _
            AccDate:=DateColumn, _
            Description:=Description)
            
        CsvStream.WriteLine csv
        
    End If
               
End Sub


Private Sub HandleTrades(ByRef Row As Range, ByRef CsvStream As TextStream, ByRef ExchangeRates As Dictionary)

    Dim csv As String
    
    Dim Header As String
    Dim DataDiscriminator As String
    Dim AssetCategory As String
    Dim CurrencySymbol As String
    Dim Symbol As String
    Dim DateTime As String
    Dim Quantity As Integer
    Dim TPrice As Currency
    Dim CPrice As Currency
    Dim Proceeds As Currency
    Dim CommFee As Currency
    Dim Basis As Currency
    Dim RealizedPL As Currency
    Dim RealizedPLPercent As Double
    Dim MtmPL As Currency
    Dim Code As String
    
    Header = Row.Cells(1, 2).Value
    DataDiscriminator = Row.Cells(1, 3).Value
    AssetCategory = Row.Cells(1, 4).Value
    CurrencySymbol = Row.Cells(1, 5).Value
    Symbol = Row.Cells(1, 6).Value
    DateTime = Row.Cells(1, 7).Value
    Quantity = Row.Cells(1, 8).Value
    TPrice = Row.Cells(1, 9).Value
    CPrice = Row.Cells(1, 10).Value
    Proceeds = Row.Cells(1, 11).Value
    CommFee = Row.Cells(1, 12).Value
    Basis = Row.Cells(1, 13).Value
    RealizedPL = Row.Cells(1, 14).Value
    RealizedPLPercent = Row.Cells(1, 15).Value
    MtmPL = Row.Cells(1, 16).Value
    Code = Row.Cells(1, 17).Value
            
    Dim AccountingDate As Date
    
    AccountingDate = DateSerial( _
        Left(DateTime, 4), _
        Mid(DateTime, 6, 2), _
        Mid(DateTime, 9, 2))

    Dim EuRate As Currency
    EuRate = GetExchangeRate(ExchangeRates, CurrencySymbol, AccountingDate)
                        
    If Basis <= 0 Then
        'Debug.Print "Trades: Skipped Row " & Row.Row & " Basis " & Basis
    ElseIf RealizedPL < 0 Then
        'Debug.Print "Trades: Skipped Row " & Row.Row & " Realized P&L " & RealizedPL
    ElseIf Quantity <= 0 Then
        'Debug.Print "Trades: Skipped Row " & Row.Row & " Quantity " & Quantity
    ElseIf RealizedPL = 0 And Quantity > 0 Then
    
        csv = FormatCSV( _
            Amount:=Round(Basis, 2), _
            CurrencySymbol:=CurrencySymbol, _
            EuRate:=EuRate, _
            AmountEUR:=Round(Basis / EuRate, 2), _
            Account1:="1349", _
            Account2:="1220", _
            AccDate:=AccountingDate, _
            Description:="Kauf von " & Quantity & " Wertpapieren von " & Symbol)
            
        CsvStream.WriteLine csv
    
    ElseIf RealizedPL > 0 And Quantity > 0 Then
                        
        csv = FormatCSV( _
            Amount:=Round(Basis, 2), _
            CurrencySymbol:=CurrencySymbol, _
            EuRate:=EuRate, _
            AmountEUR:=Round(Basis / EuRate, 2), _
            Account1:="1234567", _
            Account2:="1112222", _
            AccDate:=AccountingDate, _
            Description:=Symbol)
            
        CsvStream.WriteLine csv
        
        csv = FormatCSV( _
            Amount:=Round(Basis, 2), _
            CurrencySymbol:=CurrencySymbol, _
            EuRate:=EuRate, _
            AmountEUR:=Round(Basis / EuRate, 2), _
            Account1:="1234567", _
            Account2:="1112222", _
            AccDate:=AccountingDate, _
            Description:=Symbol)
            
        CsvStream.WriteLine csv
    
    Else
                
        'csv = FormatCSV(Amount, CurrencySymbol, EuRate, AmountEUR, Account1, Account2, AccDate, Description)
        'ts.WriteLine csv
                
    End If
    
               
End Sub

Private Sub ProcessExchangeRates(ByRef ExchangeRates As Dictionary, BaseDirectory As String)

    Dim DictUSD As Dictionary
    Set DictUSD = CreateObject("Scripting.Dictionary")
    ProcessExchangeRatesFromFile DictUSD, BaseDirectory & "\USD.csv"
    
    Dim DictGBP As Dictionary
    Set DictGBP = CreateObject("Scripting.Dictionary")
    ProcessExchangeRatesFromFile DictGBP, BaseDirectory & "\GBP.csv"
    
    ExchangeRates.Add "USD", DictUSD
    ExchangeRates.Add "GBP", DictGBP

End Sub


Private Sub ProcessExchangeRatesFromFile(ByRef ExchangeRates As Dictionary, FileName As String)

    Debug.Print "Read Exchange Rates from " & FileName & " ..."
    
    Dim WbExchangeRates As Workbook
    Set WbExchangeRates = Workbooks.Open(FileName:=FileName, Local:=False, ReadOnly:=True)
    Dim Row As Range

    For Each Row In WbExchangeRates.ActiveSheet.UsedRange.Rows
    
        Dim Period As Date
        Dim Rate As Currency
        Dim Status As String
        
        Status = Row.Cells(1, 3).Value
    
        If Status = "Normal value (A)" Then
            Period = Row.Cells(1, 1).Value
            Rate = Row.Cells(1, 2).Value
            ExchangeRates.Add Period, Rate
        End If
    
    Next Row
    
    WbExchangeRates.Close
    
    Debug.Print "Processed " & ExchangeRates.Count & "  Exchange Rates from " & FileName

End Sub

Public Function GetExchangeRate(ByRef ExchangeRates As Dictionary, CurrencySymbol As String, RateDate As Date) As Currency

    Dim Result As Currency
    Dim CurrentDate As Date
    Dim Dict As Dictionary
    
    If CurrencySymbol = "EUR" Then
        Result = 1
    Else
        Set Dict = ExchangeRates(CurrencySymbol)
        
        Result = Dict(RateDate)
        CurrentDate = RateDate
        
        While (Result = 0 And CurrentDate < Now)
            CurrentDate = DateAdd("d", 1, CurrentDate)
            Result = Dict(CurrentDate)
        Wend
    End If
        
    If Result = 0 Then
        Result = 1 ' Just to avoid division by 0 error
    End If
    
    GetExchangeRate = Result
    
End Function

Public Function FormatCSV(Amount As Currency, CurrencySymbol As String, EuRate As Currency, AmountEUR As Currency, _
    Account1 As String, Account2 As String, AccDate As Date, Description As String) As String

    Dim csv As String

    csv = Amount & ";"                              ' 001 Umsatz
    csv = csv & """S"";"                            ' 002 Soll/Haben-Kennzeichen
    csv = csv & """" & CurrencySymbol & """;"       ' 003 WKZ Umsatz
    csv = csv & """" & EuRate & """;"               ' 004 Kurs
    csv = csv & """" & AmountEUR & """;"            ' 005 Basis-Umsatz
    csv = csv & """EUR"";"                          ' 006 WKZ Basis-Umsatz
    csv = csv & """" & Account1 & """;"             ' 007 Konto
    csv = csv & """" & Account2 & """;"             ' 008 Gegenkonto (ohne BU-Schlüssel)
    csv = csv & """"";"                             ' 009 BU-Schlüssel
    csv = csv & Format(AccDate, "DDMM") & ";"       ' 010 Belegdatum
    csv = csv & """"";"                             ' 011 Belegfeld 1
    csv = csv & """"";"                             ' 012 Belegfeld 2
    csv = csv & """"";"                             ' 013 Skonto
    csv = csv & """" & Description & """;"          ' 014 Buchungstext
    csv = csv & """0"";"                            ' 015 Postensperre
    csv = csv & """"";"                             ' 016 Diverse Adressnummer
    csv = csv & """"";"                             ' 017 Geschäftspartnerbank
    csv = csv & """"";"                             ' 018 Sachverhalt
    csv = csv & """"";"                             ' 019 Zinssperre
    csv = csv & """"";"                             ' 020 Beleglink
    csv = csv & """"";"                             ' 021 Beleginfo-Art 1
    csv = csv & """"";"                             ' 022 Beleginfo-Inhalt 1
    csv = csv & """"";"                             ' 023 Beleginfo-Art 2
    csv = csv & """"";"                             ' 024 Beleginfo-Inhalt 2
    csv = csv & """"";"                             ' 025 Beleginfo-Art 3
    csv = csv & """"";"                             ' 026 Beleginfo-Inhalt 3
    csv = csv & """"";"                             ' 027 Beleginfo-Art 4
    csv = csv & """"";"                             ' 028 Beleginfo-Inhalt 4
    csv = csv & """"";"                             ' 029 Beleginfo-Art 5
    csv = csv & """"";"                             ' 030 Beleginfo-Inhalt 5
    csv = csv & """"";"                             ' 031 Beleginfo-Art 6
    csv = csv & """"";"                             ' 032 Beleginfo-Inhalt 6
    csv = csv & """"";"                             ' 033 Beleginfo-Art 7
    csv = csv & """"";"                             ' 034 Beleginfo-Inhalt 7
    csv = csv & """"";"                             ' 035 Beleginfo-Art 8
    csv = csv & """"";"                             ' 036 Beleginfo-Inhalt 8
    csv = csv & """"";"                             ' 037 KOST1
    csv = csv & """"";"                             ' 038 KOST2
    csv = csv & """"";"                             ' 039 KOST-Menge
    csv = csv & """"";"                             ' 040 EU-Mitgliedstaat u. UStID
    csv = csv & """"";"                             ' 041 EU-Steuersatz
    csv = csv & """"";"                             ' 042 Abw. Versteuerungsart
    csv = csv & """"";"                             ' 043 Sachverhalt L+L
    csv = csv & """"";"                             ' 044 Funktionsergänzung L+L
    csv = csv & """"";"                             ' 045 BU 49 Hauptfunktionstyp
    csv = csv & """"";"                             ' 046 BU 49 Hauptfunktionsnummer
    csv = csv & """"";"                             ' 047 BU 49 Funktionsergänzung
    csv = csv & """"";"                             ' 048 Zusatzinformation-Art 1
    csv = csv & """"";"                             ' 049 Zusatzinformation-Inhalt 1
    csv = csv & """"";"                             ' 050 Zusatzinformation-Art 2
    csv = csv & """"";"                             ' 051 Zusatzinformation-Inhalt 2
    csv = csv & """"";"                             ' 052 Zusatzinformation-Art 3
    csv = csv & """"";"                             ' 053 Zusatzinformation-Inhalt 3
    csv = csv & """"";"                             ' 054 Zusatzinformation-Art 4
    csv = csv & """"";"                             ' 055 Zusatzinformation-Inhalt 4
    csv = csv & """"";"                             ' 056 Zusatzinformation-Art 5
    csv = csv & """"";"                             ' 057 Zusatzinformation-Inhalt 5
    csv = csv & """"";"                             ' 058 Zusatzinformation-Art 6
    csv = csv & """"";"                             ' 059 Zusatzinformation-Inhalt 6
    csv = csv & """"";"                             ' 060 Zusatzinformation-Art 7
    csv = csv & """"";"                             ' 061 Zusatzinformation-Inhalt 7
    csv = csv & """"";"                             ' 062 Zusatzinformation-Art 8
    csv = csv & """"";"                             ' 063 Zusatzinformation-Inhalt 8
    csv = csv & """"";"                             ' 064 Zusatzinformation-Art 9
    csv = csv & """"";"                             ' 065 Zusatzinformation-Inhalt 9
    csv = csv & """"";"                             ' 066 Zusatzinformation-Art 10
    csv = csv & """"";"                             ' 067 Zusatzinformation-Inhalt 10
    csv = csv & """"";"                             ' 068 Zusatzinformation-Art 11
    csv = csv & """"";"                             ' 069 Zusatzinformation-Inhalt 11
    csv = csv & """"";"                             ' 070 Zusatzinformation-Art 12
    csv = csv & """"";"                             ' 071 Zusatzinformation-Inhalt 12
    csv = csv & """"";"                             ' 072 Zusatzinformation-Art 13
    csv = csv & """"";"                             ' 073 Zusatzinformation-Inhalt 13
    csv = csv & """"";"                             ' 074 Zusatzinformation-Art 14
    csv = csv & """"";"                             ' 075 Zusatzinformation-Inhalt 14
    csv = csv & """"";"                             ' 076 Zusatzinformation-Art 15
    csv = csv & """"";"                             ' 077 Zusatzinformation-Inhalt 15
    csv = csv & """"";"                             ' 078 Zusatzinformation-Art 16
    csv = csv & """"";"                             ' 079 Zusatzinformation-Inhalt 16
    csv = csv & """"";"                             ' 080 Zusatzinformation-Art 17
    csv = csv & """"";"                             ' 081 Zusatzinformation-Inhalt 17
    csv = csv & """"";"                             ' 082 Zusatzinformation-Art 18
    csv = csv & """"";"                             ' 083 Zusatzinformation-Inhalt 18
    csv = csv & """"";"                             ' 084 Zusatzinformation-Art 19
    csv = csv & """"";"                             ' 085 Zusatzinformation-Inhalt 19
    csv = csv & """"";"                             ' 086 Zusatzinformation-Art 20
    csv = csv & """"";"                             ' 087 Zusatzinformation-Inhalt 20
    csv = csv & """"";"                             ' 088 Stück
    csv = csv & """"";"                             ' 089 Gewicht
    csv = csv & """"";"                             ' 090 Zahlweise
    csv = csv & """"";"                             ' 091 Forderungsart
    csv = csv & """"";"                             ' 092 Veranlagungsjahr
    csv = csv & """"";"                             ' 093 Zugeordnet Fälligkeit
    csv = csv & """"";"                             ' 094 Skontotyp
    csv = csv & """"";"                             ' 095 Auftragsnummer
    csv = csv & """"";"                             ' 096 Buchungstyp
    csv = csv & """"";"                             ' 097 USt-Schlüssel (Anzahlungen)
    csv = csv & """"";"                             ' 098 EU-Mitgliedstaat (Anzahlungen)
    csv = csv & """"";"                             ' 099 Sachverhalt L+L (Anzahlungen)
    csv = csv & """"";"                             ' 100 EU-Steuersatz (Anzahlungen)
    csv = csv & """"";"                             ' 101 Erlöskonto (Anzahlungen)
    csv = csv & """"";"                             ' 102 Herkunft-Kz
    csv = csv & """"";"                             ' 103 Leerfeld
    csv = csv & """"";"                             ' 104 KOST-Datum
    csv = csv & """"";"                             ' 105 SEPA-Mandatsreferenz
    csv = csv & """"";"                             ' 106 Skontosperre
    csv = csv & """"";"                             ' 107 Gesellschaftername
    csv = csv & """"";"                             ' 108 Beteiligtennummer
    csv = csv & """"";"                             ' 109 Identifikationsnummer
    csv = csv & """"";"                             ' 110 Zeichnernummer
    csv = csv & """"";"                             ' 111 Postensperre
    csv = csv & """"";"                             ' 112 Bezeichnung SoBil-Sachverhalt
    csv = csv & """"";"                             ' 113 Kennzeichen SoBil-Buchung
    csv = csv & """"";"                             ' 114 Festschreibung
    csv = csv & """"";"                             ' 115 Leistungsdatum
    csv = csv & """"";"                             ' 116 Datum Zuordn. Steuerperiode
    csv = csv & """"";"                             ' 117 Fälligkeit
    csv = csv & """"";"                             ' 118 Generalumkehr
    csv = csv & """"";"                             ' 119 Steuersatz
    csv = csv & """"";"                             ' 120 Land
    csv = csv & """"";"                             ' 121 Abrechnungsreferenz
    csv = csv & """"";"                             ' 122 BVV-Position (Betriebsvermögensvergleich)
    csv = csv & """"";"                             ' 123 EU-Mitgliedstaat u. USt.ID (Ursprung)
    csv = csv & """"""                              ' 124 EU-Steuersatz (Ursprung)

    FormatCSV = csv

End Function

Public Function GetEXTFHeader1() As String

    Dim header1 As String
        
    header1 = """EXTF"";"                           ' Kennzeichen
    header1 = header1 & "700;"                      ' Versionsnummer
    header1 = header1 & "21;"                       ' Formatkategorie (21 = Buchungsstapel)
    header1 = header1 & """Buchungsstapel"";"       ' Formatname
    header1 = header1 & "12;"                       ' Formatversion (Buchungsstapel = 12)
    header1 = header1 & "20210130140440439;"        ' Erzeugt am (YYYYMMDDHHMMSSFFF)
    header1 = header1 & ";"                         ' Reserviert (Leerfeld)
    header1 = header1 & ";"                         ' Reserviert (Leerfeld)
    header1 = header1 & ";"                         ' Reserviert (Leerfeld)
    header1 = header1 & ";"                         ' Reserviert (Leerfeld)
    header1 = header1 & """29098"";"                ' Beraternummer (Bereich 1001-9999999)
    header1 = header1 & """55003"";"                ' Mandantennummer (Bereich 1-99999)
    header1 = header1 & """20210101"";"             ' Wirtschaftsjahresbeginn (Format: YYYYMMDD)
    header1 = header1 & "4;"                        ' Sachkontenlänge (Nummernlänge der Sachkonten)
    header1 = header1 & "20210101;"                 ' Datum von (Beginn der Periode des Stapels. Format: YYYYMMDD)
    header1 = header1 & "20210831;"                 ' Datum bis (Ende der Periode des Stapels. Format: YYYYMMDD)
    header1 = header1 & """Trading 2020"";"         ' Bezeichnung (Bezeichnung des Stapels)
    header1 = header1 & """AB"";"                   ' Diktatkürzel (Kürzel in Großbuchstaben des Bearbeiters)
    header1 = header1 & "1;"                        ' Buchungstyp (1 = Finanzbuchführung, 2 = Jahresabschluss)
    header1 = header1 & "0;"                        ' Rechnungslegungszweck (0 = unabhängig)
    header1 = header1 & "0;"                        ' Festschreibung (0 = keine Festschreibung)
    header1 = header1 & """EUR"";"                  ' WKZ (ISO-Code der Währung)
    header1 = header1 & ";"                         ' Reserviert (Leerfeld)
    header1 = header1 & ";"                         ' Derivatskennzeichen (Leerfeld)
    header1 = header1 & ";"                         ' Reserviert (Leerfeld)
    header1 = header1 & ";"                         ' Reserviert (Leerfeld)
    header1 = header1 & """03"";"                   ' Sachkontenrahmen
    header1 = header1 & ";"                         ' ID der Branchenlösung
    header1 = header1 & ";"                         ' Reserviert (Leerfeld)
    header1 = header1 & ";"                         ' Reserviert (Leerfeld)
    header1 = header1 & "Import von IB"             ' Anwendungs-Information
    
    GetEXTFHeader1 = header1

End Function

Public Function GetEXTFHeader2() As String

    Dim header2 As String
            
    header2 = "Umsatz (ohne Soll/Haben-Kz);"        ' Umsatz/Betrag für den Datensatz
    header2 = header2 & "Soll/Haben-Kennzeichen;"   ' S = SOLL (default), H = HABEN
    header2 = header2 & "WKZ Umsatz;"               ' ISO-Code der Währung
    header2 = header2 & "Kurs;"                     ' Wenn Umsatz in Fremdwährung bei #1 angegeben wird
    header2 = header2 & "Basis-Umsatz;"             ' Umsatz in der Basiswährung
    header2 = header2 & "WKZ Basis-Umsatz;"         ' Währungskennzeichen für den Basis-Umsatz
    header2 = header2 & "Konto;"                    ' Sach- oder Personenkonto
    header2 = header2 & "Gegenkonto (ohne BU-Schlüssel);" ' Sach- oder Personenkonto
    header2 = header2 & "BU-Schlüssel;"             ' Steuerungskennzeichen zur Abbildung verschiedener Funktionen/Sachverhalte
    header2 = header2 & "Belegdatum;"               ' Format: TTMM
    header2 = header2 & "Belegfeld 1;"              ' Rechnungs-/Belegnummer. Wird als "Schlüssel" für den Ausgleich offener Rechnungen genutzt
    header2 = header2 & "Belegfeld 2;"              ' http://www.datev.de/info-db/9211385
    header2 = header2 & "Skonto;"                   ' Skontobetrag
    header2 = header2 & "Buchungstext;"             ' 0-60 Zeichen
    header2 = header2 & "Postensperre;"             ' Mahn- oder Zahlsperre: 0 = keine Sperre (default), 1 = Sperre
    header2 = header2 & "Diverse Adressnummer;"     ' Adressnummer einer diversen Adresse.
    header2 = header2 & "Geschäftspartnerbank;"     ' Referenz um für Lastschrift oder Zahlung eine bestimmte Geschäftspartnerbank genutzt werden soll.
    header2 = header2 & "Sachverhalt;"              ' Kennzeichen für einen Mahnzins/Mahngebühr-Datensatz (31 = Mahnzins, 40 = Mahngebühr)
    header2 = header2 & "Zinssperre;"               ' Sperre für Mahnzinsen (0 = keine Sperre (default), 1 = Sperre)
    header2 = header2 & "Beleglink;"                ' Link zu einem digitalen Beleg in einer DATEV App
    header2 = header2 & "Beleginfo - Art 1;"        ' https://developer.datev.de/portal/de/dtvf/formate/buchungsstapel
    header2 = header2 & "Beleginfo - Inhalt 1;"
    header2 = header2 & "Beleginfo - Art 2;"
    header2 = header2 & "Beleginfo - Inhalt 2;"
    header2 = header2 & "Beleginfo - Art 3;"
    header2 = header2 & "Beleginfo - Inhalt 3;"
    header2 = header2 & "Beleginfo - Art 4;"
    header2 = header2 & "Beleginfo - Inhalt 4;"
    header2 = header2 & "Beleginfo - Art 5;"
    header2 = header2 & "Beleginfo - Inhalt 5;"
    header2 = header2 & "Beleginfo - Art 6;"
    header2 = header2 & "Beleginfo - Inhalt 6;"
    header2 = header2 & "Beleginfo - Art 7;"
    header2 = header2 & "Beleginfo - Inhalt 7;"
    header2 = header2 & "Beleginfo - Art 8;"
    header2 = header2 & "Beleginfo - Inhalt 8;"
    header2 = header2 & "KOST1 - Kostenstelle;" ' Zuordnung des Geschäftsvorfalls für die anschließende Kostenrechnung
    header2 = header2 & "KOST2 - Kostenstelle;" ' Zuordnung des Geschäftsvorfalls für die anschließende Kostenrechnung
    header2 = header2 & "Kost-Menge;"           ' z. B. kg, g, cm, m, %
    header2 = header2 & "EU-Land u. UStID (Bestimmung);"    ' https://www.datev.de/info-db/9211462
    header2 = header2 & "EU-Steuersatz (Bestimmung);"       ' Der im EU-Bestimmungsland gültige Steuersatz
    header2 = header2 & "Abw. Versteuerungsart;"            ' I=Ist-Versteuerung, K=keine Umsatzsteuerrechnung, P=Pauschalierung (z. B. für Land- und Forstwirtschaft), S=Soll-Versteuerung
    header2 = header2 & "Sachverhalt L+L;"                  ' Sachverhalte gem. § 13b Abs. 1 Satz 1 Nrn. 1.-5. UStG
    header2 = header2 & "Funktionsergänzung L+L;"           ' Steuersatz / Funktion zum L+L-Sachverhalt
    header2 = header2 & "BU 49 Hauptfunktionstyp;"          ' https://developer.datev.de/portal/de/dtvf/formate/buchungsstapel
    header2 = header2 & "BU 49 Hauptfunktionsnummer;"
    header2 = header2 & "BU 49 Funktionsergänzung;"
    header2 = header2 & "Zusatzinformation - Art 1;"
    header2 = header2 & "Zusatzinformation- Inhalt 1;"
    header2 = header2 & "Zusatzinformation - Art 2;"
    header2 = header2 & "Zusatzinformation- Inhalt 2;"
    header2 = header2 & "Zusatzinformation - Art 3;"
    header2 = header2 & "Zusatzinformation- Inhalt 3;"
    header2 = header2 & "Zusatzinformation - Art 4;"
    header2 = header2 & "Zusatzinformation- Inhalt 4;"
    header2 = header2 & "Zusatzinformation - Art 5;"
    header2 = header2 & "Zusatzinformation- Inhalt 5;"
    header2 = header2 & "Zusatzinformation - Art 6;"
    header2 = header2 & "Zusatzinformation- Inhalt 6;"
    header2 = header2 & "Zusatzinformation - Art 7;"
    header2 = header2 & "Zusatzinformation- Inhalt 7;"
    header2 = header2 & "Zusatzinformation - Art 8;"
    header2 = header2 & "Zusatzinformation- Inhalt 8;"
    header2 = header2 & "Zusatzinformation - Art 9;"
    header2 = header2 & "Zusatzinformation- Inhalt 9;"
    header2 = header2 & "Zusatzinformation - Art 10;"
    header2 = header2 & "Zusatzinformation- Inhalt 10;"
    header2 = header2 & "Zusatzinformation - Art 11;"
    header2 = header2 & "Zusatzinformation- Inhalt 11;"
    header2 = header2 & "Zusatzinformation - Art 12;"
    header2 = header2 & "Zusatzinformation- Inhalt 12;"
    header2 = header2 & "Zusatzinformation - Art 13;"
    header2 = header2 & "Zusatzinformation- Inhalt 13;"
    header2 = header2 & "Zusatzinformation - Art 14;"
    header2 = header2 & "Zusatzinformation- Inhalt 14;"
    header2 = header2 & "Zusatzinformation - Art 15;"
    header2 = header2 & "Zusatzinformation- Inhalt 15;"
    header2 = header2 & "Zusatzinformation - Art 16;"
    header2 = header2 & "Zusatzinformation- Inhalt 16;"
    header2 = header2 & "Zusatzinformation - Art 17;"
    header2 = header2 & "Zusatzinformation- Inhalt 17;"
    header2 = header2 & "Zusatzinformation - Art 18;"
    header2 = header2 & "Zusatzinformation- Inhalt 18;"
    header2 = header2 & "Zusatzinformation - Art 19;"
    header2 = header2 & "Zusatzinformation- Inhalt 19;"
    header2 = header2 & "Zusatzinformation - Art 20;"
    header2 = header2 & "Zusatzinformation- Inhalt 20;"
    header2 = header2 & "Stück;"
    header2 = header2 & "Gewicht;"
    header2 = header2 & "Zahlweise;"                        ' 1 = Lastschrift, 2 = Mahnung, 3 = Zahlung
    header2 = header2 & "Forderungsart;"
    header2 = header2 & "Veranlagungsjahr;"
    header2 = header2 & "Zugeordnete Fälligkeit;"
    header2 = header2 & "Skontotyp;"
    header2 = header2 & "Auftragsnummer;"
    header2 = header2 & "Buchungstyp;"
    header2 = header2 & "USt-Schlüssel (Anzahlungen);"
    header2 = header2 & "EU-Land (Anzahlungen);"
    header2 = header2 & "Sachverhalt L+L (Anzahlungen);"
    header2 = header2 & "EU-Steuersatz (Anzahlungen);"
    header2 = header2 & "Erlöskonto (Anzahlungen);"
    header2 = header2 & "Herkunft-Kz;"
    header2 = header2 & "Buchungs GUID;"
    header2 = header2 & "KOST-Datum;"
    header2 = header2 & "SEPA-Mandatsreferenz;"
    header2 = header2 & "Skontosperre;"
    header2 = header2 & "Gesellschaftername;"
    header2 = header2 & "Beteiligtennummer;"
    header2 = header2 & "Identifikationsnummer;"
    header2 = header2 & "Zeichnernummer;"
    header2 = header2 & "Postensperre bis;"
    header2 = header2 & "Bezeichnung SoBil-Sachverhalt;"
    header2 = header2 & "Kennzeichen SoBil-Buchung;"
    header2 = header2 & "Festschreibung;"
    header2 = header2 & "Leistungsdatum;"
    header2 = header2 & "Datum Zuord. Steuerperiode;"
    header2 = header2 & "Fälligkeit;"
    header2 = header2 & "Generalumkehr (GU);"
    header2 = header2 & "Steuersatz;"
    header2 = header2 & "Land;"
    header2 = header2 & "Abrechnungsreferenz;"
    header2 = header2 & "BVV-Position;"
    header2 = header2 & "EU-Land u. UStID (Ursprung);"
    header2 = header2 & "EU-Steuersatz(Ursprung)"
    
    GetEXTFHeader2 = header2

End Function


