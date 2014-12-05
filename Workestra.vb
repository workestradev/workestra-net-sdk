'
' Copyright (c) 2014 Workestra LLC
' All rights reserved.
'
' Redistribution and use in source and binary forms, with or without modification,
' are permitted provided that the following conditions are met:
'
' * Redistributions of source code must retain the above copyright notice, this
' list of conditions and the following disclaimer.
'
' * Redistributions in binary form must reproduce the above copyright notice,
' this list of conditions and the following disclaimer in the documentation
' and/or other materials provided with the distribution.
'
' * Neither the name of Workestra nor the names of its contributors may be used
' to endorse or promote products derived from this software without specific
' prior written permission.
'
'
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
' ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
' DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
' FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
' DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
' SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
' CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
' OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
'

Imports System
Imports System.Collections.Specialized
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Uri

Module Workestra

    Public Class WorkestraHTTPRequest
        Public method As String = ""
        Public headers As NameValueCollection
        Public url As String = ""
        Public contents As String = ""

        Public Sub New()
            headers = New NameValueCollection()
        End Sub
    End Class

    Public Class WorkestraHTTPResponse
        Public responseCode As Integer
        Public headers As NameValueCollection
        Private content As HttpWebResponse

        Sub New(ByVal responseCode As Integer)
            Me.responseCode = responseCode
        End Sub

        Public Sub New(ByVal content As HttpWebResponse)
            Me.content = content
        End Sub


        Public Function getContentBytes() As Byte()

            If Not content Is Nothing Then
                Dim inStream As Stream = content.GetResponseStream()

                Dim buffer(32768) As Byte
                Using ms As MemoryStream = New MemoryStream()

                    While (True)
                        Dim read As Integer = inStream.Read(buffer, 0, buffer.Length)
                        If (read <= 0) Then
                            ms.Close()
                            content.Close()
                            Return ms.ToArray()
                        End If
                        ms.Write(buffer, 0, read)
                    End While

                End Using
                Return buffer

            Else
                Dim empty() As Byte = {}
                Return empty
            End If
        End Function


        Public Function getContentString() As String
            If (Not content Is Nothing) Then

                Dim contentReader As StreamReader = New StreamReader(content.GetResponseStream())
                Dim ret As String = contentReader.ReadToEnd()


                contentReader.Close()
                content.Close()
                Return ret
            Else
                Return ""
            End If
        End Function

    End Class


    Public Class WorkestraHTTPClient
        Private basicAuthUsername As String = ""
        Private basicAuthPassword As String
        Private verifyCert As Boolean = True

        Public Sub setBasicAuth(ByVal username As String, ByVal password As String)
            basicAuthUsername = username
            basicAuthPassword = password
        End Sub

        Public Sub setCertificateValidation(Optional ByVal verifyCert As Boolean = True)
            Me.verifyCert = verifyCert
        End Sub


        Public Function sendRequest(ByVal req As WorkestraHTTPRequest) As WorkestraHTTPResponse
            Dim request As HttpWebRequest = WebRequest.Create(req.url)
            request.KeepAlive = False
            request.Method = req.method

            Dim iCount As Integer = req.headers.Count
            Dim key As String
            Dim keyvalue As String

            Dim i As Integer
            For i = 0 To iCount - 1
                key = req.headers.Keys(i)
                keyvalue = req.headers(i)
                request.Headers.Add(key, keyvalue)
            Next

            Dim enc As System.Text.UTF8Encoding = New System.Text.UTF8Encoding()
            Dim bytes() As Byte = {}

            If (req.contents.Length > 0) Then
                bytes = enc.GetBytes(req.contents)
                request.ContentLength = bytes.Length
            End If

            request.AllowAutoRedirect = False

            If Not basicAuthUsername.Equals("") Then
                Dim authInfo As String = basicAuthUsername + ":" + basicAuthPassword
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo))
                request.Headers("Authorization") = "Basic " + authInfo
            End If


            If req.contents.Length > 0 Then
                Dim outBound As Stream = request.GetRequestStream()
                outBound.Write(bytes, 0, bytes.Length)
            End If

            Dim resp As WorkestraHTTPResponse
            Try

                Dim webresponse As HttpWebResponse = request.GetResponse()
                resp = New WorkestraHTTPResponse(webresponse)
                resp.responseCode = webresponse.StatusCode
                resp.headers = webresponse.Headers
            Catch e As WebException

                If (e.Status = WebExceptionStatus.ProtocolError) Then
                    resp = New WorkestraHTTPResponse(DirectCast(e.Response, HttpWebResponse).StatusCode)
                Else
                    resp = New WorkestraHTTPResponse(0)
                End If
            End Try

            Return resp
        End Function


    End Class


    Public Class WorkestraAPIClient
        Private http As WorkestraHTTPClient
        Public baseUrl As String = "https://www.workestra.co/api/v1"


        Public Sub New(Optional ByVal client As WorkestraHTTPClient = Nothing)

            If Not client Is Nothing Then
                http = client
            Else
                http = New WorkestraHTTPClient()
            End If

        End Sub

        Public Sub setApiKey(ByVal key As String)
            http.setBasicAuth(key, "w")
        End Sub

         Public Sub setBasicAuth(ByVal username As String, ByVal password As String)
            http.setBasicAuth(key, password)
        End Sub



        Public Function listNotifications(Optional ByVal fields() As String = "") As WorkestraHTTPResponse
            Dim request As WorkestraHTTPRequest = New WorkestraHTTPRequest()
            request.method = "GET"
            request.url = String.Format("/notifications", _
                                        Me.baseUrl, employeeId, String.Join(",", fields))

            Return http.sendRequest(request)
        End Function



    End Class


    

End Module
