Imports System.Threading

Public Class FormMain
    Private segmentLength As Integer = 20
    Private probability As Double = 0.43
    Private delay As Integer = 1000 / 60

    Private surface As Bitmap
    Private syncObj As New Object()
    Private y As Integer

    Private gSurface As Graphics

    Private fColor As New Pen(Color.White, 2.5)
    Private bColor As New SolidBrush(Color.FromArgb(0, 0, 170))

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.UserPaint, True)

        AddHandler Me.Resize, Sub() CreateSurface()

        CreateSurface()
        Task.Run(Sub()
                     Dim rnd As New Random()
                     Do
                         SyncLock syncObj
                             Dim ys As Integer = y + segmentLength

                             For x As Integer = 0 To surface.Width - segmentLength - 1 Step segmentLength
                                 If rnd.NextDouble() <= probability Then
                                     gSurface.DrawLine(fColor, x, y, x + segmentLength, ys) ' \
                                 Else
                                     gSurface.DrawLine(fColor, x + segmentLength, y, x, ys) ' /
                                 End If
                             Next

                             If y + segmentLength >= surface.Height Then
                                 y -= segmentLength
                                 gSurface.DrawImageUnscaled(surface, 0, -segmentLength)
                                 gSurface.FillRectangle(bColor, 0, y, surface.Width, segmentLength)
                             Else
                                 y += segmentLength
                             End If
                         End SyncLock

                         Thread.Sleep(delay * 6)
                     Loop
                 End Sub)

        Task.Run(Sub()
                     Do
                         Me.Invalidate()
                         Thread.Sleep(delay)
                     Loop
                 End Sub)
    End Sub

    Private Sub CreateSurface()
        SyncLock syncObj
            y = 0

            surface = New Bitmap(Me.DisplayRectangle.Width, Me.DisplayRectangle.Height + segmentLength)
            gSurface = Graphics.FromImage(surface)

            gSurface.SmoothingMode = Drawing2D.SmoothingMode.None
            gSurface.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor

            gSurface.Clear(bColor.Color)
        End SyncLock
    End Sub

    Private Sub FormMain_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        SyncLock syncObj
            e.Graphics.DrawImageUnscaled(surface, 0, 0)
        End SyncLock
    End Sub
End Class