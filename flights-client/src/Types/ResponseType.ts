import { FlightResponse } from "./FlightTypes";

export interface ApiHeader {
  ReturnCode: string;
  ReturnCodeMessage: string;
}
export interface FlightsApiResponse {
  TicketResponse: string;
  Header: ApiHeader;
  Body: FlightResponse[];
  InterchangeId: string | null;
}