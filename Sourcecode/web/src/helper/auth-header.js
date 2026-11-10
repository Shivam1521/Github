import { refreshTokenIfNeeded, getToken } from "./authManager";

export function authHeader() {
  // return authorization header with jwt token

  let token = sessionStorage.getItem("token");
  const Curenturl = window.location.pathname;
  const nNumber = Curenturl.lastIndexOf("/");
  const pageName = Curenturl.substring(nNumber + 1);

  if (token) {
    return {
      Authorization: "Bearer " + token,
      page: pageName,
      "Content-Type": "application/json",
      Accept: "application/json",
      "X-Frame-Options": "Deny",
      "X-XSS-Protection": "1",
      "X-Content-Type-Options": "nosniff",
      "cache-control": "max-age=3153600",
    };
  } else {
    sessionStorage.clear();
    window.location = "/login";
  }
}

export function authHeaderNoToken() {
  const Curenturl = window.location.pathname;
  const nNumber = Curenturl.lastIndexOf("/");
  const pageName = Curenturl.substring(nNumber + 1);
  return {
    page: pageName,
    "Content-Type": "application/json",
    Accept: "application/json",
    "X-Frame-Options": "Deny",
    "X-XSS-Protection": "1",
    "X-Content-Type-Options": "nosniff",
    "cache-control": "max-age=3153600",
  };
}

export function authHeaderFormData() {
  // return authorization header with jwt token

  let token = sessionStorage.getItem("token");
  const Curenturl = window.location.pathname;
  const nNumber = Curenturl.lastIndexOf("/");
  const pageName = Curenturl.substring(nNumber + 1);

  if (token) {
    return {
      Authorization: "Bearer " + token,
      page: pageName,
      //"Content-Type": "multipart/form-data",
      Accept: "application/json",
      "X-Frame-Options": "Deny",
      "X-XSS-Protection": "1",
      "X-Content-Type-Options": "nosniff",
      "cache-control": "max-age=3153600",
    };
  } else {
    sessionStorage.clear();
    window.location = "/login";
  }
}

export async function handleResponse(response, originalRequest) {
  const token = sessionStorage.getItem("token");
  const refreshToken = sessionStorage.getItem("refreshToken");
  const curl = window.location.pathname;

  if (response.status === 418 && !curl.includes("/login")) {
    return {
      ResponseMessage: "User unauthorized, please contact admin.",
      ResponseCode: 418,
    };
  }

  if (
    response.status === 401 &&
    originalRequest &&
    !originalRequest.url.includes("/Auth/Login")
  ) {
    try {
      const newToken = await refreshTokenIfNeeded();
      const retryOptions = {
        ...originalRequest,
        headers: {
          ...originalRequest.headers,
          Authorization: `Bearer ${newToken}`,
        },
      };
      const retryResponse = await fetch(originalRequest.url, retryOptions);
      const retryText = await retryResponse.text();
      return JSON.parse(retryText);
    } catch {
      return {
        responseCode: "401",
        responseMessage: "Unauthorized",
        responseData: null,
      };
    }
  }

  // Normal response
  const text = await response.text();
  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}
