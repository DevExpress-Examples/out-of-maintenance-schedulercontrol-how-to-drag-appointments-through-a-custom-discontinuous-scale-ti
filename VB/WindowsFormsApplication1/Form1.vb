Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler

Namespace WindowsFormsApplication1
    Partial Public Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
        End Sub

        Private minTime As New TimeSpan(8, 0, 0)
        Private maxTime As New TimeSpan(19, 0, 0)

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim scales As TimeScaleCollection = schedulerControl1.TimelineView.Scales
            schedulerControl1.TimelineView.Scales.BeginUpdate()
            Try
                scales.Clear()
                scales.Add(New MyTimeScaleFixedInterval(TimeSpan.FromDays(1), minTime, maxTime))
                scales.Add(New MyTimeScaleFixedInterval(TimeSpan.FromHours(1), minTime, maxTime))
                scales.Add(New MyTimeScaleFixedInterval(TimeSpan.FromMinutes(30), minTime, maxTime))
            Finally
                schedulerControl1.TimelineView.Scales.EndUpdate()
            End Try

            Dim apt As Appointment = schedulerStorage1.CreateAppointment(AppointmentType.Normal)
            apt.Start = Date.Today.AddHours(10)
            apt.End = Date.Today.AddHours(14)
            apt.Subject = "Test appointment"
            schedulerStorage1.Appointments.Add(apt)
        End Sub

        Private Sub CorrectAppointmentParameters(ByVal e As AppointmentDragEventArgs)
            Dim customScaleDuration As TimeSpan = (maxTime.Subtract(minTime)).Add(TimeSpan.FromHours(2))
            Dim initialStart As Date = e.EditedAppointment.Start

            If e.EditedAppointment.Start.TimeOfDay < minTime OrElse e.EditedAppointment.Start.TimeOfDay >= maxTime Then
                Dim durationMustBeIncreased As Boolean = e.EditedAppointment.Duration > customScaleDuration
                e.EditedAppointment.Start = If(durationMustBeIncreased, e.EditedAppointment.Start + customScaleDuration, e.EditedAppointment.Start - customScaleDuration)

                Dim appInterval As New TimeInterval(e.EditedAppointment.Start, e.EditedAppointment.Start + e.EditedAppointment.Duration + customScaleDuration)
                If Not appInterval.Contains(e.HitInterval) Then
                    durationMustBeIncreased = Not durationMustBeIncreased
                    e.EditedAppointment.Start = If(durationMustBeIncreased, initialStart.Add(customScaleDuration), initialStart.Subtract(customScaleDuration))
                End If
                e.Handled = True
            End If

            If e.EditedAppointment.End.TimeOfDay >= maxTime OrElse e.EditedAppointment.End.TimeOfDay < minTime Then
                e.EditedAppointment.End = If(e.EditedAppointment.Duration > customScaleDuration, e.EditedAppointment.End - customScaleDuration, e.EditedAppointment.End + customScaleDuration)
                e.Handled = True
            End If
        End Sub

        Private Sub schedulerControl1_AppointmentDrag(ByVal sender As Object, ByVal e As AppointmentDragEventArgs) Handles schedulerControl1.AppointmentDrag
            CorrectAppointmentParameters(e)
        End Sub

        Private Sub schedulerControl1_AppointmentDrop(ByVal sender As Object, ByVal e As AppointmentDragEventArgs) Handles schedulerControl1.AppointmentDrop
            CorrectAppointmentParameters(e)
        End Sub
    End Class
End Namespace
