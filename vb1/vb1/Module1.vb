Module Module1

    Sub Main()
        Dim n756 = "123412345678901234567"
        Dim d756 = DAC756(n756)

        Dim n748 = "123456789012"
        Dim d748 = Calculo_DV11_748(n748)

        Dim n001 = "12345678922"
        Dim d001 = DAC001(n001)


        Dim n033 = "00000000000000000000000000000000000001234567"
        Dim r033 = DAC033(n033)

        Dim n237 = DAC237(1234567890124)

        Dim x = DACLD("00571234511012345678", 1)

        Dim y = 0
    End Sub

    Public Function DACLD(ByVal Codigo As String, ByVal Multip As Byte) As Long
        Dim i As Long
        Dim lngSoma As Long
        Dim lngDAC As Long
        Dim strDigitos As String
        Dim arrDados As Object


        If Len(Codigo) = 0 Then
            DACLD = 0
            Exit Function
        End If

        ReDim arrDados(Len(Codigo))

        For i = 1 To Len(Codigo)
            arrDados(i) = (Val(Mid(Codigo, i, 1)) * Multip)

            Multip = IIf(Multip = 1, 2, 1)
        Next

        strDigitos = Join(arrDados, "")

        lngSoma = 0
        For i = 1 To Len(strDigitos)
            lngSoma = lngSoma + Val(Mid(strDigitos, i, 1))
        Next

        DACLD = (lngSoma Mod 10)
        DACLD = 10 - DACLD

        If DACLD >= 10 Then
            DACLD = 0
        End If
    End Function

    Private Function DAC237(dblNum As Double) As String
        Dim strNum As String
        Dim strMult As String
        Dim dblRes As Double
        Dim i As Long

        If dblNum = 0 Then
            Exit Function
        End If

        strNum = Format(dblNum, "0000000000000")
        strMult = "2765432765432"

        dblNum = 0
        For i = 13 To 1 Step -1
            dblNum = dblNum + Val(Mid(strNum, i, 1)) * Val(Mid(strMult, i, 1))
        Next

        dblRes = dblNum Mod 11

        If dblRes = 0 Then
            DAC237 = "0"
        Else
            DAC237 = Trim(Str(11 - dblRes))
            If DAC237 = 10 Then
                DAC237 = "P"
            End If
        End If

    End Function

    Function DAC033(strNumero As String) As String
        'declara as variáveis
        Dim intcontador As Integer
        Dim intnumero As Integer
        Dim intTotalNumero As Integer
        Dim intMultiplicador As Integer
        Dim intResto As Integer

        On Error GoTo Erro

        '-> Se nao for um valor numerico sai da função
        If Not IsNumeric(strNumero) Then
            DAC033 = ""
            Exit Function
        End If

        'inicia o multiplicador
        intMultiplicador = 2

        'pega cada caracter do numero a partir da direita
        For intcontador = Len(strNumero) To 1 Step -1

            'extrai o caracter e multiplica prlo multiplicador
            intnumero = Val(Mid(strNumero, intcontador, 1)) * intMultiplicador

            'soma o resultado para totalização
            intTotalNumero = intTotalNumero + intnumero

            'se o multiplicador for maior que 2 decrementa-o caso contrario atribuir valor padrao original
            intMultiplicador = intMultiplicador + 1
            If intMultiplicador > 8 Then intMultiplicador = 2

        Next

        'calcula o resto da divisao do total por 11
        intResto = intTotalNumero Mod 11

        'verifica as exceções ( 0 -> DV=0    10 -> DV=X (para o BB) e retorna o DV
        Select Case intResto
            Case 0
                DAC033 = "0"
            Case 1
                DAC033 = "0"
            Case Else
                DAC033 = Str(11 - intResto)
        End Select

Fim:
        Exit Function
Erro:
        'MsgBox Traduzir("Erro ao calcular Digito verificador") & vbCrLf & Traduzir("Erro:") & Err.Number & " - " & Err.Description, vbCritical, "DV"
        Resume Fim
    End Function

    Function DAC001(strNum As String) As String
        Dim strMult As String
        Dim i As Long

        'Possui 21 digitos a formação do nosso número.
        'strNum = Format(dblNum, "00000000000")

        'Constante para calculo do nosso número = 3197.
        strMult = "78923456789"

        Dim dblNum = 0
        For i = 11 To 1 Step -1
            dblNum = dblNum + Val(Mid(strNum, i, 1)) * Val(Mid(strMult, i, 1))
        Next

        DAC001 = dblNum Mod 11
    End Function

    Function Calculo_DV11_748(strNumero As String) As String
        'declara as variáveis
        Dim intcontador As Integer
        Dim intnumero As Integer
        Dim intTotalNumero As Integer
        Dim intMultiplicador As Integer
        Dim intResto As Integer

        On Error GoTo Erro

        ' se nao for um valor numerico sai da função
        If Not IsNumeric(strNumero) Then
            Calculo_DV11_748 = ""
            Exit Function
        End If

        'inicia o multiplicador
        intMultiplicador = 2

        'pega cada caracter do numero a partir da direita
        For intcontador = Len(strNumero) To 1 Step -1

            'extrai o caracter e multiplica prlo multiplicador
            intnumero = Val(Mid(strNumero, intcontador, 1)) * intMultiplicador

            'soma o resultado para totalização
            intTotalNumero = intTotalNumero + intnumero

            'se o multiplicador for maior que 2 decrementa-o caso contrario atribuir valor padrao original
            intMultiplicador = IIf(intMultiplicador > 8, 2, intMultiplicador + 1)

        Next

        'calcula o resto da divisao do total por 11
        intResto = intTotalNumero Mod 11
        If intResto <= 1 Then
            Calculo_DV11_748 = 0
        Else
            Calculo_DV11_748 = 11 - intResto
        End If

Fim:
        Exit Function
Erro:
        Resume Fim
    End Function

    Private Function DAC756(dblNum As String) As String
        '**************************************************
        'MODULO 11 DO NOSSO NUMERO DO BOLETO SICOOB-COOP.
        '**************************************************
        'ENTRADA: NOSSO NUMERO DO BOLETO
        '  SAÍDA: DIGITO VERIF. DO NOSSO NUMERO
        '  AUTOR: JOELSON ROCHA
        'CRIAÇÃO: 20/08/2015 11:15hrs.
        '**************************************************

        Dim strNum As String
        Dim strMult As String
        Dim dblRes As Double
        Dim i As Long

        If dblNum = 0 Then
            Exit Function
        End If

        'Possui 21 digitos a formação do nosso número.
        'strNum = Format(dblNum, "000000000000000000000")
        strNum = "123412345678901234567"


        'Constante para calculo do nosso número = 3197.
        strMult = "319731973197319731973"

        dblNum = 0
        For i = 21 To 1 Step -1
            dblNum = dblNum + Val(Mid(strNum, i, 1)) * Val(Mid(strMult, i, 1))
        Next

        dblRes = dblNum Mod 11

        If dblRes <= 1 Then
            DAC756 = "0"
        Else
            DAC756 = Trim(Str(11 - dblRes))
            If DAC756 >= 10 Then
                DAC756 = "0"
            End If
        End If

    End Function


End Module
