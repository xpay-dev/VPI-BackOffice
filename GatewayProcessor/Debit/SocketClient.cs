using HPA_ISO8583;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.Debit
{
    public class SocketClient
    {
        public SwitchResponse CallSynchronousSocketClient(string inputRequest)
        {
            SwitchResponse response = new SwitchResponse();

            string emailSupport = ConfigurationManager.AppSettings["EmailSupport"].ToString();
            string ip = ConfigurationManager.AppSettings["PATHMAXBANK"].ToString();
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["PORTMAXBANK"]);
            string result = string.Empty;
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.

                //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.SendTimeout = 180000; //3 minutes //180000
                    //sender.Connect(remoteEP);

                    var resultConnect = sender.BeginConnect(remoteEP, null, null);

                    bool success = resultConnect.AsyncWaitHandle.WaitOne(180000, true);

                    if (!success)
                    {
                        response.Status = "Timeout";
                        response.ErrNumber = "100";
                        response.Message = "Connection Timeout";

                        sender.EndConnect(resultConnect);
                    }

                    //else
                    //{
                    //    sender.Close();
                    //    throw new SocketException(10060); // Connection timed out.
                    //}

                    Console.WriteLine("Socket connected to {0}",sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes(inputRequest);

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);

                    result = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    DE_ISO8583 res = new ISO8583().Parse(result);
  
                    response.AuthorizationID = res.AUTHORIZATION_IDENTIFICATION_RESPONSE;
                    response.ReturnCode = res.RESPONSE_CODE;
                    response.SytemTraceAudit = res.SYSTEM_TRACE_AUDIT_NUMBER;
                    response.Reference = res.RETRIEVAL_REFERENCE_NUMBER;
                    response.BalanceInqData = res.ADDITIONAL_AMOUNTS;

                    response.ErrNumber = res.RESPONSE_CODE;
                    response.Message = res.RESERVED_NATIONAL_57;
                    
                    if(response.ReturnCode == "00")
                    {
                        response.Status = "Approved";
                    }
                    else
                    {
                        if(response.ReturnCode == "99")
                        {
                            response.Message = "Cannot connect to client server. Please contact support at " + emailSupport;
                        }
                        else if(response.ReturnCode == "98")
                        {
                            response.Message = "Unreachable client hsm. Please contact support at " + emailSupport;
                        }
                        else if (response.ReturnCode == "97")
                        {
                            response.Message = "Unable to parse data. Please contact support at " + emailSupport;
                        }
                        else
                        {
                            response.Message += " Please contact support at " + emailSupport;
                        }

                        response.Status = "Declined";
                    }

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    response.Status = "Declined";
                    response.ErrNumber = "101";
                    response.Message = "ArgumentNullException : " + ane.Message;
                }
                catch (SocketException se)
                {
                    response.Status = "Declined";
                    response.ErrNumber = "102";
                    response.Message = "SocketException : " + "Cannot Connect to Service API.";
                }
                catch (Exception e)
                {
                    response.Status = "Declined";
                    response.ErrNumber = "103";
                    response.Message = "Unexpected exception  : " + "Error occurs while connecting to service.";
                }
            }
            catch (Exception ex)
            {
                response.Status = "Declined";
                response.ErrNumber = "104";
                response.Message = "Cannot Connect to Service API.";
            }

            return response;
        }
    }
}
