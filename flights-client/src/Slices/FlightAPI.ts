import { FlightsApiResponse } from "../Types/ResponseType";
const API_BASE = "https://localhost:7172/api/Flights";

export const getAllFlights = async () => {
  const res = await fetch(`${API_BASE}/GetAllFlights`);
    const json: FlightsApiResponse = await res.json();
    return json 

};

export const getFlightById = async (id: string) => {
  const res = await fetch(`${API_BASE}/GetFlightById/${id}`);
  return res.json();
};

export const insertFlight = async (flight: any) => {
  const res = await fetch(`${API_BASE}/InsertFlight`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(flight),
  });
  return res.json();
};

export const updateFlight = async (flight: any) => {
  const res = await fetch(`${API_BASE}/UpdateFlight/${flight.guid}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(flight),
  });
  return res.json();
};

export const deleteFlight = async (id: string) => {
  const res = await fetch(`${API_BASE}/DeleteFlight/${id}`, {
    method: "DELETE",
  });
  return res.json();
};

export const searchByQuery = async (query: string) => {
  const res = await fetch(`${API_BASE}/Search?${query}`);
  const json: FlightsApiResponse = await res.json();
  return json; 
};