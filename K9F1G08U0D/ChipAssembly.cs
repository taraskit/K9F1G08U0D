using NAND_Prog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K9F1G08U0D
{
    // This dll compiles all the components of the K9F1G08U0D chip (via the DI interface) and implements the DI interface to NAND_Prog.exe



    #region Help
    /*  
    ��������� , ���� ������� � NAND_Prog (�� �������� (����������� ��� ��������� NAND_Prog.���)):
      ��������� ��� ����������� ������� :

       [Import("NAND_Prog.Organization", typeof(MemoryOrg))]         - ����������� �����

       [Import("NAND_Prog.Bytes per page", typeof(MemoryOrg))]       - ���� � �������

       [Import("NAND_Prog.Spare bytes per page", typeof(MemoryOrg))] - ���������� ���� � �������

       [Import("NAND_Prog.Pages per block", typeof(MemoryOrg))]      - ������� ������� � �����

       [Import("NAND_Prog.Bloks per LUN", typeof(MemoryOrg))]        - ������� ����� � ��

       [Import("NAND_Prog.LUNs", typeof(MemoryOrg))]                 - ������� �� � ���

       [Import("NAND_Prog.Column address cycles", typeof(MemoryOrg))] - ������� ����� ��� ��������� �������

       [Import("NAND_Prog.Row address cycles", typeof(MemoryOrg))]    - ������� ����� ��� ��������� �����



       [Import("NAND_Prog.Device Manufacturing" , typeof(Chip))]      - �������� ����

       [Import("NAND_Prog.Chip name", typeof(Chip))]                  - ��� ����


       <int>("NAND_Prog.Bad Block Mark")                              - ������� �� bad blok


        
       [Import("ChipDependency", typeof(List<string>))]               - ��������� ���� (������ dll-�� � ���� ���������� ���)

      �� ��������� ������� :

        [ImportMany("NAND_Prog.Chip", typeof(Operation))]             - ���� �������� ��� ����� ����            

        [ImportMany("NAND_Prog.Sub part", typeof(ChipPart))]          - ���� ���������� ������ , �� � � ����� ���� (��������� ID Register , Status Register � ���.)

        [Import("NAND_Prog.activeSR", typeof(SRregister), AllowDefault = true)]  -  ������-������ ��� ������ ������� �������� Programm i Erase

        [Import("Interpreted" ,typeof(SRregister), AllowDefault = true)]         - ������������� ����� ������-�������

        [Import("NAND_Prog.Algo", typeof(AlgoMapBB), AllowDefault = true)]       - �������� ������ ��� �����


        ---------------------------------------------------------------------------------------------------------------------------------

        ����� ����� private object objX ���������� ����������� ��� (����������� � �� �����������) ������� ��� NAND_Prog.exe .
        ̳����� �������� � ������ [Import] ��� ����� ����� objX ����� �������� ��������� �� �� ���� ��������� ���������� Importa � NAND_Prog.exe
        ������� ������� ��� ����� ���� ��������� ����� ��������� � ��� ��������� � ������ [Import] �������� � DLL � ��� ���� ���� �����������

     */
    #endregion

    public class ChipDependency
    {
        [Export("ChipDependency", typeof(List<string>))]
        private List<string> chip_dep;

        public ChipDependency()
        {
            chip_dep = new List<string>();

            chip_dep.Add("BadBlockImplement.dll");
        
            chip_dep.Add("ChipErase.dll");
            chip_dep.Add("ChipProgramm.dll");
            chip_dep.Add("ChipRead.dll");
            chip_dep.Add("ChipReset.dll");

            chip_dep.Add("ID_Interpreted.dll");
            chip_dep.Add("ID_Read.dll");
            chip_dep.Add("ID_Register.dll");

            chip_dep.Add("SR_Interpreted.dll");
            chip_dep.Add("SR_Read.dll");
            chip_dep.Add("StatusRegister.dll");

        }
    }


    #region Requared
    #region Don't edit !!!

    // DO NOT EDIT THIS CODE    !!!!
    public class ChipStructure
    {       

        [Export("NAND_Prog.Device Manufacturing", typeof(NAND_Prog.Chip))]
   //     [Import("ChipDescriptor.Device Manufacturing", typeof(NAND_Prog.Chip))]
        private object devManuf;


        [Export("NAND_Prog.Chip name", typeof(NAND_Prog.Chip))]
    //    [Import("ChipDescriptor.Chip name", typeof(NAND_Prog.Chip))]
        private object name;


      

        //-----------------------------------------------------

        [Export("MemoryOrg.Organization", typeof(MemoryOrg))]               
        public Organization width;

        [Export("MemoryOrg.Bytes per page", typeof(MemoryOrg))]
        public int bytesPP;

        [Export("MemoryOrg.Spare bytes per page", typeof(MemoryOrg))]
        public UInt16 spareBytesPP;

        [Export("MemoryOrg.Pages per block", typeof(MemoryOrg))]
        public UInt32 pagesPB;

        [Export("MemoryOrg.Bloks per LUN", typeof(MemoryOrg))]
        public UInt32 bloksPLUN;

        [Export("MemoryOrg.LUNs", typeof(MemoryOrg))]
        public byte LUNs;

        [Export("MemoryOrg.Column address cycles", typeof(MemoryOrg))]
        public byte colAdrCycles;

        [Export("MemoryOrg.Row address cycles", typeof(MemoryOrg))]
        public byte rowAdrCycles;
        
        #endregion


        public ChipStructure()
        {
         

            devManuf = "SAMSUNG";
            name = "K9F1G08U0D";

            width = Organization.x8;
            bytesPP = 0x0800;      // ����� ������� - 2048 ���� (2Kb)
            spareBytesPP = 0x40;   // ����� Spare Area - 64 ����
            pagesPB = 0x40;        // ������� ������� � ����� - 64 
            bloksPLUN = 0x0400;    // ������� ����� � CE - 1024
            LUNs = 0x01;           // ������� CE � ���
            colAdrCycles = 0x02;   // ��������� ������� 
            rowAdrCycles = 0x03;   // ��������� �����

            

        }
    }

    public class BadBolockImplement
    {
        //����������� ������� GetExportedValue<int>("BadBlockProvider.BadBlockMark")  � NAND_Prog.exe     
        //---------------------------------------------------------------
        [Export("BadBlockProvider.BadBlockMark", typeof(int))]
        [Import("SomeDll.BadBlockMark", typeof(int))]
        private object obj1;
    }


    #endregion


    #region Not requared
    public class ChipOperation
    {
        //����������� ������� [ImportMany("NAND_Prog.Chip", typeof(Operation))]   � NAND_Prog.exe     
        //---------------------------------------------------------------

        [Export("NAND_Prog.Chip", typeof(List<Operation>))]
        [Import("ChipReset", typeof(Operation))]
        private object obj1;

        [Export("NAND_Prog.Chip", typeof(List<Operation>))]
        [Import("ChipRead", typeof(Operation))]
        private object obj2;

        [Export("NAND_Prog.Chip", typeof(List<Operation>))]
        [Import("ChipProgramm", typeof(Operation))]
        private object obj3;

        [Export("NAND_Prog.Chip", typeof(List<Operation>))]
        [Import("ChipErase", typeof(Operation))]
        private object obj4;

        //-------------------------------------------------------------------
    }


    
    public class ChipSubParts
    {
        //����������� ������� [ImportMany("NAND_Prog.Sub part", typeof(ChipPart))]   � NAND_Prog.exe     
        //---------------------------------------------------------------

        //  [Export("NAND_Prog.Sub part", typeof(ChipPart))]
        [Export("Chip.Sub parts", typeof(ChipPart))]
        [Import("ID Register", typeof(ChipPart))]
        public object _id_register;

        //--------------------------------------------------

        [Export("Chip.Sub parts", typeof(ChipPart))]
        [Export("Chip.activeSR", typeof(SRregister))]                        //���������� ����� �� �� � ������-������ ��� �������� ���������� �������� 
        [Import("Status Register", typeof(ChipPart))]
        public object _sr_register;

        

    }

    
    public class Setup_ID_Register
    {

        [Export("ID Register name", typeof(string))]
        string ID_Register_name = "ID register";

        [Export("ID Register size", typeof(int))]
        int ID_Register_size = 0x05;


        //����������� ������� [ImportMany("ID Operation", typeof(Operation))] ��� ID Register

        [Export("ID Register Operation", typeof(Operation))]
        [Import("ID Read", typeof(Operation))]
        private object obj1;


        //����������� ������� [Import("ID_Interpreted", typeof(Register), AllowDefault = true)]  ��� ID Register
        [Export("ID Register Interpreted", typeof(Register))]
        [Import("K9F1G08U0D_IDInterpreted", typeof(Register))]
        private object obj2;
    }

    public class Setup_Status_Register
    {
        #region Required


        [Export("Status Register name", typeof(string))]
        string Status_Register_name = "Status Register";

        [Export("Status Register size", typeof(int))]
        int Status_Register_size = 0x01;

        #endregion

        #region Not required (Option)
        
        //����������� ������� [ImportMany("Status Register Operations", typeof(Operation))]   for   Status Register
        [Export("Status Register Operations", typeof(Operation))]
        [Import("Status Read", typeof(Operation))]
        private object obj1;

        [Export("Status Register Interpreted", typeof(Func<Operation, SRregister, bool?>))]

        #endregion


    }

    /*
    public class StatusRegisterImports
    {
        //����������� ������� [ImportMany("SR_operations", typeof(Operation))] ��� Status Register
        [Export("SR_operations", typeof(Operation))]
        [Import("SR_Read", typeof(Operation))]
        private object obj1;

        //����������� ������� [Import("SR_Interpreted", typeof(SRregister), AllowDefault = true)]  ��� Status Register
        [Export("SR_Interpreted", typeof(SRregister))]
        [Import("SRInterpreted", typeof(SRregister))]
        private object obj2;

    }

    */
    #endregion
}
