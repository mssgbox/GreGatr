Imports System.ServiceModel.Syndication

Public Class ScoreComparer
    Implements System.Collections.Generic.IComparer(Of SyndicationItem)
    Public Function Compare(ByVal x As SyndicationItem, ByVal y As SyndicationItem) As Integer Implements IComparer(Of System.ServiceModel.Syndication.SyndicationItem).Compare


        Dim higherScoreOne As Integer = _
            GetHigherScore(DirectCast(XElement.ReadFrom(x.ElementExtensions(0).GetReader()), XElement).Value, _
                            DirectCast(XElement.ReadFrom(x.ElementExtensions(1).GetReader()), XElement).Value) '(RTScore, MetaScore)
        Dim higherScoreTwo As Integer = _
            GetHigherScore(DirectCast(XElement.ReadFrom(y.ElementExtensions(0).GetReader()), XElement).Value, _
                            DirectCast(XElement.ReadFrom(y.ElementExtensions(1).GetReader()), XElement).Value) '(RTScore, MetaScore)

        If higherScoreOne > higherScoreTwo Then
            Return 1
        ElseIf higherScoreOne = higherScoreTwo Then
            Return 0
        Else
            Return -1
        End If

    End Function

    Private Function GetHigherScore(ByVal x As String, ByVal y As String) As Integer
        'initialized to zeros
        Dim higherScore As Integer
        Dim numberOne, numberTwo As Integer

        Int32.TryParse(x, numberOne)
        Int32.TryParse(y, numberTwo)

        'could be zeros
        If numberOne > numberTwo Then
            higherScore = numberOne
        Else
            higherScore = numberTwo
        End If

        Return higherScore
    End Function


End Class
