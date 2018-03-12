using System.Collections.Generic;
using CashCenter.Objective.DocumentsReceipt.Dal;
using CashCenter.ViewCommon;

namespace CashCenter.Objective.DocumentsReceipt
{
    public class DocumentReceiptControlViewModel : ViewModel
    {
        public IEnumerable<Department> Departments
        {
            get
            {
                // TODO: implement
                return new[]
                {
                    new Department { Id = 1, Name = "dep1" },
                    new Department { Id = 2, Name = "dep2" }
                };
            }
        }

        public IEnumerable<SettlementCenter> SettlementCenters
        {
            get
            {
                // TODO: implement
                return new[]
                {
                    new SettlementCenter { Id = 1, Name = "center1" },
                    new SettlementCenter { Id = 2, Name = "center2" }
                };
            }
        }

        public IEnumerable<ReceiptDocument> PreloadedDocuments
        {
            get
            {
                // TODO: implement
                return new List<ReceiptDocument>();
            }
        }

        public ViewProperty<string> SelectedFilename { get; private set; }
        public Command AddDeparmentCommand { get; private set; }
        public Command AddSettlementCenterCommand { get; private set; }
        public Command AddPreloadedDocumentCommand { get; private set; }
        public Command DropDocumentCommand { get; private set; }
        public Command LoadPreloadedDocumentsCommand { get; private set; }

        public DocumentReceiptControlViewModel()
        {
            SelectedFilename = new ViewProperty<string>("SelectedFilename", this);
            AddDeparmentCommand = new Command(AddDeparmentHandler);
            AddSettlementCenterCommand = new Command(AddSettlementCenterHandler);
            AddPreloadedDocumentCommand = new Command(AddPreloadedDocumentHandler);
            DropDocumentCommand = new Command(DropDocumentHandler);
            LoadPreloadedDocumentsCommand = new Command(LoadPreloadedDocumentsHandler);
        }

        private void AddDeparmentHandler(object parameters)
        {
            // TODO: implement
            Message.Info("Add department");
        }

        private void AddSettlementCenterHandler(object parameters)
        {
            // TODO: implement
            Message.Info("Add settlement center");
        }

        private void AddPreloadedDocumentHandler(object parameters)
        {
            // TODO: implement
            Message.Info("Add preloaded documents");
        }

        private void DropDocumentHandler(object parameters)
        {
            // TODO: implement
            Message.Info("Drop document");
        }

        private void LoadPreloadedDocumentsHandler(object parameters)
        {
            // TODO: implement
            Message.Info("Load preloaded documents");
        }
    }
}
