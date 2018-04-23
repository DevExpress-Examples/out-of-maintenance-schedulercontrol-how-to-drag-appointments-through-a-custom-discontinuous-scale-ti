Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.XtraScheduler

Namespace WindowsFormsApplication1
    Public Class MyTimeScaleFixedInterval
        Inherits TimeScaleFixedInterval

        Private start As TimeSpan
        Private [end] As TimeSpan

        Public Sub New(ByVal span As TimeSpan, ByVal spanStart As TimeSpan, ByVal spanEnd As TimeSpan)
            MyBase.New(span)
            Me.Width = 40
            start = spanStart
            [end] = spanEnd
        End Sub

        Public Overrides Function Floor(ByVal [date] As Date) As Date
            If [date].TimeOfDay < start Then
                Return [date].Date.AddDays(-1) + [end] - Value
            End If
            If [date].TimeOfDay = start Then
                Return [date]
            End If
            If [date].TimeOfDay = [end] Then
                Return [date]
            End If
            Dim result As Date = MyBase.Floor([date])
            If result.TimeOfDay > [end] Then
                Return [date].Date + [end]
            End If
            If result.TimeOfDay < start Then
                Return result.Date + start
            End If
            Return result
        End Function

        Public Overrides Function GetNextDate(ByVal [date] As Date) As Date
            If [date].TimeOfDay = start Then
                [date] = MyBase.Floor([date])
            End If
            Dim result As Date = MyBase.GetNextDate([date])
            If result.TimeOfDay >= [end] Then
                Return result.Date.AddDays(1) + start
            End If
            If result.TimeOfDay <= start Then
                Return result.Add(start)
            End If
            Return result
        End Function

        Public Overrides Function FormatCaption(ByVal start As Date, ByVal [end] As Date) As String
            If MyBase.Value = TimeSpan.FromDays(1) Then
                Return start.ToString("d ddd")
            End If
            Return start.ToString("HH:mm")
        End Function
    End Class
End Namespace
