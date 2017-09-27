Imports System.Threading

Public Class FormMain
    Private segmentLength As Integer = 20
    Private probability As Double = 0.43
    Private vertSpeed As Integer = 2
    Private delay As Integer = 1000 / 60

    Private surface As Bitmap
    Private syncObj As New Object()
    Private y As Integer

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.UserPaint, True)

        Dim refresh As New Thread(Sub()
                                      Do
                                          Me.Invalidate()
                                          Thread.Sleep(delay)
                                      Loop
                                  End Sub) With {
            .IsBackground = True
        }
        refresh.Start()

        AddHandler Me.Resize, Sub() CreateSurface()

        CreateSurface()
        Dim draw As New Thread(Sub()
                                   Dim rnd As New Random()
                                   Do
                                       SyncLock syncObj
                                           Using g As Graphics = Graphics.FromImage(surface)
                                               g.SmoothingMode = Drawing2D.SmoothingMode.None
                                               g.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor

                                               For x As Integer = 0 To surface.Width - segmentLength - 1 Step segmentLength
                                                   If rnd.NextDouble() <= probability Then
                                                       g.DrawLine(Pens.White, x, y, x + segmentLength, y + segmentLength)
                                                   Else
                                                       g.DrawLine(Pens.White, x + segmentLength, y, x, y + segmentLength)
                                                   End If
                                               Next

                                               If y + segmentLength >= surface.Height Then
                                                   y -= vertSpeed
                                                   g.DrawImageUnscaled(surface, 0, -vertSpeed)
                                                   g.FillRectangle(Brushes.Black, 0, y, surface.Width, segmentLength * 2)
                                               Else
                                                   y += segmentLength
                                               End If
                                           End Using
                                       End SyncLock

                                       Thread.Sleep(delay)
                                   Loop
                               End Sub) With {
            .IsBackground = True
        }
        draw.Start()
    End Sub

    Private Sub CreateSurface()
        SyncLock syncObj
            y = 0

            surface = New Bitmap(Me.DisplayRectangle.Width, Me.DisplayRectangle.Height + segmentLength * 2)
            Using g As Graphics = Graphics.FromImage(surface)
                g.Clear(Color.Black)
            End Using
        End SyncLock
    End Sub

    Private Sub FormMain_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        SyncLock syncObj
            e.Graphics.DrawImageUnscaled(surface, 0, 0)
        End SyncLock
    End Sub
End Class