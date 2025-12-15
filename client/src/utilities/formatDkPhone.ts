export const formatDkPhone = (phone?: string | null) => {
  if (!phone) return "";

  // keep only digits
  const digits = phone.replace(/\D/g, "");

  // remove country code if already present
  const local = digits.startsWith("45") ? digits.slice(2) : digits;

  if (local.length !== 8) return phone; // fallback if unexpected length

  return `+45 ${local.slice(0, 2)} ${local.slice(2, 4)} ${local.slice(4, 6)} ${local.slice(6, 8)}`;
};
