let refreshingTokenPromise = null;

export function getToken() {
  return sessionStorage.getItem("token");
}

export function setTokens(token, refreshToken) {
  sessionStorage.setItem("token", token);
  sessionStorage.setItem("refreshToken", refreshToken);
}

export function clearSessionAndRedirect() {
  sessionStorage.clear();
  window.location = "/login";
}

export function refreshTokenIfNeeded() {
  if (!refreshingTokenPromise) {
    const token = getToken();
    const refreshToken = sessionStorage.getItem("refreshToken");

    refreshingTokenPromise = fetch(
      `${import.meta.env.VITE_API_URL}/Auth/Refresh`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          AccessToken: token,
          RefreshToken: refreshToken,
        }),
      },
    )
      .then(async (res) => {
        if (!res.ok) {
          sessionStorage.clear();
          window.location = "/login";
          throw new Error("Refresh failed");
        }
        const data = await res.json();

        const newToken = data?.responseData?.token;
        const newRefresh = data?.responseData?.refreshToken;

        if (newToken && newRefresh) {
          setTokens(newToken, newRefresh);
          return newToken;
        }

        throw new Error("Invalid refresh response");
      })
      .catch((err) => {
        clearSessionAndRedirect();
        throw err;
      })
      .finally(() => {
        refreshingTokenPromise = null;
      });
  }

  return refreshingTokenPromise;
}
