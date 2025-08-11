export interface Flight {
  flightNumber: string;
  destination: string;
  departureTime: string;
  gate: string;
  createDate: string;

}
export interface FlightResponse extends Flight {
      guid: string;
      status: string;
}
