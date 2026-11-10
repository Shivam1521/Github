import { authHeader, handleResponse } from "../helper/auth-header";

export default class DashboardService {
  async GetDashboardData(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };
    const url = `${import.meta.env.VITE_API_URL}/Dashboard/GetDashboardData`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }

 async GetDashboardProductionAnalysis(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };
    const url = `${import.meta.env.VITE_API_URL}/Dashboard/GetDashboardProductionAnalysis`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }


  async GetDashboardOverview(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };
    const url = `${import.meta.env.VITE_API_URL}/Dashboard/GetDashboardOverview`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }

  async GetDashboardOverviewFilters(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };
    const url = `${import.meta.env.VITE_API_URL}/Dashboard/GetDashboardOverviewFilters`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }


}


