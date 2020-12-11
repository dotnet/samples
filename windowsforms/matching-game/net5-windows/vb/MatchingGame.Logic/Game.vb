Imports System.Collections.Generic

Public NotInheritable Class Game

    Private ReadOnly _cards() As Char
    Private ReadOnly _openCards() As Boolean
    Private ReadOnly _turnCards As New List(Of Integer)(2)

    Private _turns As Integer

    Private Sub New(width As Integer, height As Integer, board As Char())
        Me.Width = width
        Me.Height = height
        _cards = board
        ReDim _openCards(_cards.Length - 1)
    End Sub

    Public Shared Function Create() As Game
        Dim numberOfPairs = 8
        Dim width = numberOfPairs / 2
        Dim height = width

        Dim board((width * height) - 1) As Char
        Dim availableCards = New List(Of Char) From
        {
            "!"c, "!"c, "N"c, "N"c, ","c, ","c, "k"c, "k"c,
            "b"c, "b"c, "v"c, "v"c, "w"c, "w"c, "z"c, "z"c
        }

        Dim Random As New Random()

        For i = 0 To numberOfPairs * 2 - 1
            Dim cardIndex = random.Next(availableCards.Count - 1)
            Dim card = availableCards(cardIndex)
            board(i) = card
            availableCards.RemoveAt(cardIndex)
        Next

        Return New Game(width, height, board)
    End Function

    Public ReadOnly Property Turns As Integer
        Get
            Return _turns
        End Get
    End Property

    Public ReadOnly Property Width
    Public ReadOnly Property Height
    Public ReadOnly Property RemainingCardsInTurn
        Get
            Return 2 - _turnCards.Count
        End Get
    End Property

    Private Function GetIndex(w As Integer, h As Integer) As Integer
        Return h * Width + w
    End Function

    Public Function OpenCard(w As Integer, h As Integer) As Integer
        If _turnCards.Count > 1 Then
            Throw New InvalidOperationException("Cannot open more than two cards.")
        End If

        Dim cardIndex = GetIndex(w, h)
        Dim cardValue = _cards(cardIndex)

        _turnCards.Add(cardIndex)
        _openCards(cardIndex) = True

        If _turnCards.Count = 2 Then
            _turns += 1
        End If

        Return AscW(cardValue)
    End Function

    Public Sub CloseCards()
        For Each cardIndex In _turnCards
            _openCards(cardIndex) = False
        Next
    End Sub

    Public Function GetCard(w As Integer, h As Integer) As Char
        Dim cardIndex = GetIndex(w, h)
        Return _cards(cardIndex)
    End Function

    Public Function IsOpen(w As Integer, h As Integer) As Boolean
        Dim cardIndex = GetIndex(w, h)
        Return _openCards(cardIndex)
    End Function

    Public Function CompleteTurn() As Boolean
        If _turnCards.Count <> 2 Then
            Return False
        End If

        Dim firstCardIndex = _turnCards(0)
        Dim secondCardIndex = _turnCards(1)
        Dim isMatch = _cards(firstCardIndex) = _cards(secondCardIndex)

        If Not isMatch Then
            CloseCards()
        End If

        _turnCards.Clear()

        Return isMatch
    End Function

    Public Function IsComplete() As Boolean
        For Each isVisible In _openCards
            If Not isVisible Then
                Return False
            End If
        Next

        Return True
    End Function
End Class
