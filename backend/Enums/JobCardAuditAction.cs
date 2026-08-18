namespace IThelpdesk.Enums
{
    public enum JobCardAuditAction
    {
        // Ticket Events
        JobCardCreated,
        TicketAssigned,
        TechnicianReassigned,
        TicketEscalated,
        TicketResolved,
        TicketReopened,
        TicketStatusChanged,

        // Job Card Events
        JobCardOpened,
        JobCardCompleted,
        JobCardDeleted,

        StatusChanged,

        FaultFoundUpdated,
        WorkPerformedUpdated,
        CompletionNotesUpdated,

        LabourAdded,
        LabourUpdated,
        LabourDeleted,

        CustomerSignatureAdded,
        PartAdded,
        PartUpdated,
        PartDeleted,
        PartNameUpdated,
        PartQuantityUpdated,
        PdfGenerated
    }
}
