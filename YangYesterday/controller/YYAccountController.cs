using System;
using System.Collections.Generic;
using YangYesterday.entity;
using YangYesterday.model;
using YangYesterday.utility;

namespace YangYesterday.controller
{
    public class YYAccountController
    {
        private YYAccount account = new YYAccount();
        private YYAccountModel model = new YYAccountModel();

        public bool Register()
        {
            YYAccount yyAccount = GetAccountInformation();
            Dictionary<string, string> errors = yyAccount.CheckValidate();
            if (errors.Count > 0)
            {
                Console.WriteLine("Please fix errros below and try again.");
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }

                return false;
            }
            else
            {
                // Lưu vào database.
                yyAccount.EncryptPassword();
                model.Save(yyAccount);
                return true;
            }
        }

        /** Xử lý đăng nhập người dùng.
         *  1. Yêu cầu người dùng nhập thông tin đăng nhập.
         *  2. Kiểm tra thông tin username người dùng vừa nhập vào.
         *  3.
        **/
        public bool Login()
        {
            // Yêu cầu nhập thông tin đăng nhập.
            Console.WriteLine("----------------LOGIN INFORMATION----------------");
            Console.WriteLine("Username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            // Tìm trong database thông tin tài khoản với username vừa nhập vào.
            // Trả về null nếu không tồn tại tài khoản trùng với username trên.
            // Trong trường hợp tồn tại bản ghi thì trả về thông tin account của
            // bản ghi đấy.
            YYAccount existingAccount = model.GetByUsername(username);
            // Nếu trả về null thì hàm login trả về false.
            if (existingAccount == null)
            {
                return false;
            }
            // Nếu chạy đến đây rồi thì `existingAccount` chắc chắn khác null.
            // Tiếp tục kiểm tra thông tin password.
            // Mã hoá password người dùng vừa nhập vào kèm theo muối lấy trong database
            // của bản ghi và so sánh với password đã mã hoá trong database.
            if (!existingAccount.CheckEncryptedPassword(password))
            {    
                // Nếu không trùng thì trả về false, đăng nhập thất bại.
                return false;
            }
            // Trong trường hợp chạy đến đây thì thông tin tài khoản chắc chắn khác null
            // và password đã trùng nhau. Đăng nhập thành công.
            // Lưu thông tin vừa lấy ra trong database vào biến
            // `currentLoggedInYyAccount` của lớp Program.
            Program.currentLoggedInYyAccount = existingAccount;
            // Hàm trả về true, login thành công.
            return true;
        }

        private YYAccount GetAccountInformation()
        {
            Console.WriteLine("----------------REGISTER INFORMATION----------------");
            Console.WriteLine("Username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            Console.WriteLine("Confirm Password: ");
            var cpassword = Console.ReadLine();
            Console.WriteLine("Balance: ");
            var balance = Utility.GetDecimalNumber();
            Console.WriteLine("Identity Card: ");
            var identityCard = Console.ReadLine();
            Console.WriteLine("Full Name: ");
            var fullName = Console.ReadLine();
            Console.WriteLine("Birthday: ");
            var birthday = Console.ReadLine();
            Console.WriteLine("Gender (1. Male |2. Female| 3.Others): ");
            var gender = Utility.GetInt32Number();
            Console.WriteLine("Email: ");
            var email = Console.ReadLine();
            Console.WriteLine("Phone Number: ");
            var phoneNumber = Console.ReadLine();
            Console.WriteLine("Address: ");
            var address = Console.ReadLine();
            var yyAccount = new YYAccount
            {
                Username = username,
                Password = password,
                Cpassword = cpassword,
                IdentityCard = identityCard,
                Gender = gender,
                Balance = balance,
                Address = address,
                Dob = birthday,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber
            };
            return yyAccount;
        }

        public void ShowAccountInformation()
        {
            var currentAccount = model.GetByUsername(Program.currentLoggedInYyAccount.Username);
            if (currentAccount == null)
            {
                Program.currentLoggedInYyAccount = null;
                Console.WriteLine("Sai thông tin tài khoản hoặc tài khoản đã bị khóa.");
                return;
            }

            Console.WriteLine("Số tài khoản:");
            Console.WriteLine(Program.currentLoggedInYyAccount.AccountNumber);
            Console.WriteLine("Số dư hiện tại.");
            Console.WriteLine(Program.currentLoggedInYyAccount.Balance);
        }

        public void Transfer()
        {            
            Console.WriteLine("Vui lòng nhập số tài khoản người nhận.");
            var receiverAccountNumber = Console.ReadLine();
            var checkAc = model.GetByAccountNumber(receiverAccountNumber);
            Console.WriteLine("Full Name: " + checkAc.FullName);
            Console.WriteLine("Vui lòng nhập số tiền cần chuyển: ");
            var amount = Utility.GetDecimalNumber();
            Console.WriteLine("Vui lòng nhập nội dung tin nhắn: ");
            var content = Console.ReadLine();
            var historyTransaction = new YYTransaction()
            {
                Id = Guid.NewGuid().ToString(),
                Type = YYTransaction.TransactionType.TRANSFER,
                Amount = amount,
                Content = content,
                SenderAccountNumber = Program.currentLoggedInYyAccount.AccountNumber,
                ReceiverAccountNumber = receiverAccountNumber,
                Status = YYTransaction.ActiveStatus.DONE
            };
            if (model.UpdateTranfers(Program.currentLoggedInYyAccount.AccountNumber, receiverAccountNumber, historyTransaction))
            {
                Console.WriteLine("Giao dịch thành công!");
            }
            else
            {
                Console.WriteLine("Giao dịch thất bại, vui lòng kiểm tra lại.!");
            }

        }

        public void Deposit()
        {
            Console.WriteLine("Vui lòng nhập số tiền gửi:");
            var amount = Utility.GetDecimalNumber();
            Console.WriteLine("Vui lòng nhập nội dung:");
            var content = Console.ReadLine();
            YYTransaction historyTransaction = new YYTransaction
            {
                Id = Guid.NewGuid().ToString(),
                Type = YYTransaction.TransactionType.DEPOSIT,
                Amount = amount,
                Content = content,
                SenderAccountNumber = Program.currentLoggedInYyAccount.AccountNumber,
                ReceiverAccountNumber = Program.currentLoggedInYyAccount.AccountNumber,
                Status = YYTransaction.ActiveStatus.DONE

            };
            if (model.UpdateBalance(Program.currentLoggedInYyAccount, historyTransaction))
            {
                Console.WriteLine("Giao dịch thành công!");
            }
            else
            {
                Console.WriteLine("Giao dịch thất bại, vui lòng kiểm tra lại.");
            }
            Program.currentLoggedInYyAccount = model.GetByAccountNumber(Program.currentLoggedInYyAccount.Username);
            Console.WriteLine("Số dư hiện tại: " + Program.currentLoggedInYyAccount.Balance);
            Console.WriteLine("Nhấn Enter để tiếp tục!");
            Console.ReadLine();
        }

        public void Withdraw()
        {
            Console.WriteLine("Vui lòng nhập số tiền cần rút:");
            var amount = Utility.GetDecimalNumber();
            Console.WriteLine("Vui lòng nhập nội dung:");
            var content = Console.ReadLine();
            YYTransaction historyTransaction = new YYTransaction
            {
                Id = Guid.NewGuid().ToString(),
                Type = YYTransaction.TransactionType.WITHDRAW,
                Amount = amount,
                Content = content,
                SenderAccountNumber = Program.currentLoggedInYyAccount.AccountNumber,
                ReceiverAccountNumber = Program.currentLoggedInYyAccount.AccountNumber,
                Status = YYTransaction.ActiveStatus.DONE

            };
            if (model.UpdateBalance(Program.currentLoggedInYyAccount, historyTransaction))
            {
                Console.WriteLine("Giao dịch thành công!");
            }
            else
            {
                Console.WriteLine("Giao dịch thất bại, vui lòng kiểm tra lại.!");
            }
            Program.currentLoggedInYyAccount = model.GetByAccountNumber(Program.currentLoggedInYyAccount.Username);
            Console.WriteLine("Số dư hiện tại: " + Program.currentLoggedInYyAccount.Balance);
            Console.WriteLine("nhấn Enter để tiếp tục!");
            Console.ReadLine();

        }
        
    }
}