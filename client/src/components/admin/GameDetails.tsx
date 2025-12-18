import {useParams} from "react-router";
import toast, {Toaster} from "react-hot-toast";
import {useTranslation} from "react-i18next";
import {useEffect, useState} from "react";
import getGameClient from "@core/clients/gameClient.ts";
import {
    CheckCheck,
    PartyPopper,
    Trophy,
    Users,
    Wallet,
    X
} from "lucide-react";
import {type GameDto, type NumberOfPhysicalPlayersDto, type WinningNumbersDto} from "@core/generated-client.ts";
import gameClient from "@core/clients/gameClient.ts";

const REQUIRED_NUMBERS = 3;

export default function GameDetails() {
    const {id} = useParams<{ id: string }>();
    const {t} = useTranslation();
    const [game, setGame] = useState<GameDto>();
    const [selected, setSelected] = useState<number[]>([]);
    const [isUpdateGameWinningNumbersModalOpen, setUpdateGameWinningNumbersModalOpen] = useState(false);
    const [isDrawGameWinnersModalOpen, setDrawGameWinnersModalOpen] = useState(false);
    const [isCalculateWinningsModalOpen, setCalculateWinningsModalOpen] = useState(false);
    const [numberOfPhysicalPlayers, setNumberOfPhysicalPlayers] = useState<number>(0);
    const [winningsPerPlayer, setWinningsPerPlayer] = useState<number>(0);

    const fetchGame = () => {
        const client = getGameClient();

        client.getGameById(id!)
            .then(res => {
                setGame(res)
                setNumberOfPhysicalPlayers(res.numberOfPhysicalPlayers!)
            }).catch(res =>
            toast.error("Something went wrong fetching the game")
        );
    }

    const toggle = (num: number) => {
        if (selected.includes(num)) {
            setSelected((prev) => prev.filter((x) => x !== num));
        } else {
            if (selected.length >= REQUIRED_NUMBERS) {
                toast.error(t('please_pick_three_numbers'));
                return;
            }
            setSelected((prev) => [...prev, num].sort((a, b) => a - b));
        }
    };

    useEffect(() => {
        fetchGame();
    }, [])

    function getIsoWeekRange(year: number, week: number) {
        const jan4 = new Date(Date.UTC(year, 0, 4));
        const day = jan4.getUTCDay() || 7;
        const mondayWeek1 = new Date(jan4);
        mondayWeek1.setUTCDate(jan4.getUTCDate() - day + 1);

        const start = new Date(mondayWeek1);
        start.setUTCDate(mondayWeek1.getUTCDate() + (week - 1) * 7);

        const end = new Date(start);
        end.setUTCDate(start.getUTCDate() + 6);

        return {start, end};
    }

    const weekRange = game
        ? getIsoWeekRange(game.year, game.weekNumber)
        : null;

    const format = (d: Date) =>
        d.toLocaleDateString(undefined, {
            day: "2-digit",
            month: "short",
            year: "numeric",
        });

    const handleOpenUpdateGameWinningNumbersModal = () => {
        setUpdateGameWinningNumbersModalOpen(true);
    }

    const handleCloseUpdateGameWinningNumbersModal = () => {
        setUpdateGameWinningNumbersModalOpen(false);
    }

    const handleUpdateGameWinningNumbers = () => {
        const client = getGameClient();
        const winningNumbersDto: WinningNumbersDto = {winningNumbers: selected.join(",")};

        client.updateWinningNumbers(game?.id!, winningNumbersDto)
            .then(res => {
                fetchGame();
                handleCloseUpdateGameWinningNumbersModal();
                setSelected([]);
                toast.success(t("game:numbers_updated_successfully"))
            }).catch(res => console.log(res))
    }

    const handleOpenDrawGameWinnersModal = () => {
        setDrawGameWinnersModalOpen(true);
    }

    const handleCloseDrawGameWinnersModal = () => {
        setDrawGameWinnersModalOpen(false);
    }

    const handleDrawGameWinners = () => {
        const client = gameClient();

        client.drawWinners(game?.id!)
            .then(res => {
                fetchGame();
                toast.success(t('game:winners_drawn_successfully'))
                handleCloseDrawGameWinnersModal();
            })
            .catch(res => console.log(res))
    }

    const handleOpenCalculateWinningsModal = () => {
        setCalculateWinningsModalOpen(true);
    }

    const handleCloseCalculateWinningsModal = () => {
        setCalculateWinningsModalOpen(false);
    }

    const handleCalculateWinnings = () => {
        const client = gameClient();
        const numberOfPhysicalPlayersDto: NumberOfPhysicalPlayersDto = {numberOfPhysicalPlayers: numberOfPhysicalPlayers};
        client.updateNumberOfPhysicalPlayers(game?.id!, numberOfPhysicalPlayersDto)
            .then(res => console.log(res))
            .catch(res => console.log(res))
    }

    const calculateWinningsPerPlayer = () => {

    }

    return (
        <div>
            <Toaster position="top-center"/>
            <div className={"flex flex-row items-center justify-between w-full pb-8"}>
                <div className={"flex flex-col items-baseline"}>
                    <p className={"text-white text-3xl mb-2 mx-0 p-0"}>{t("game:game_details")} - {t('week')} {game?.weekNumber} / {game?.year}</p>
                    <p className="text-sm text-gray-400 mt-0 pt-0">{weekRange && `${format(weekRange.start)} – ${format(weekRange.end)}`}</p>
                </div>
                <div className={"flex flex-row items-center gap-4"}>
                    <button
                        onClick={handleOpenDrawGameWinnersModal}
                        className={
                            "flex flex-row items-center justify-evenly px-3 py-1.5 rounded-lg bg-[#e30613] hover:bg-[#c20510] text-white shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer text-lg"
                        }
                    >
                        <Trophy size={20} className={"me-2"}/>
                        <span>{t("game:draw_winners")}</span>
                    </button>
                </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8 text-white">
                <div
                    className="bg-gray-800 rounded-2xl p-5 border border-gray-700 flex flex-col items-baseline justify-between">
                    <div className={"w-full"}>
                        <div className="flex items-start justify-between mb-4">
                            <div className="text-4xl font-bold uppercase">
                                {game?.winningNumbers ?? t('tba')}
                            </div>
                            <div className="p-3 rounded-xl bg-blue-900/30">
                                <PartyPopper className="w-6 h-6 text-blue-400"/>
                            </div>
                        </div>
                        {/*{game?.drawDate == null && (*/}
                        <div>
                            <button
                                onClick={handleOpenUpdateGameWinningNumbersModal}
                                className={"w-full mt-2 flex flex-row items-center justify-evenly px-3 py-1.5 rounded-lg bg-[#e30613] hover:bg-[#c20510] text-white shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer"}>
                                {game?.winningNumbers == null ? t('game:pick_now') : t('game:update_numbers')}
                            </button>
                        </div>
                        {/*)}*/}
                    </div>
                    <p className="text-sm mt-3 text-gray-400">{t("game:winning_numbers")}</p>
                </div>

                <div
                    className="bg-gray-800 rounded-2xl p-5 border border-gray-700 flex flex-col items-baseline justify-between">
                    <div className={"w-full"}>
                        <div className="flex items-start justify-between mb-4">
                            <div className="text-4xl font-bold">
                                {game?.revenue ?? 0}
                                <span className={"text-base"}> dkk</span>
                            </div>
                            <div className="p-3 rounded-xl bg-purple-900/30">
                                <Wallet className="w-6 h-6 text-purple-400"/>
                            </div>
                        </div>
                        <div>Jerne IF: {game?.revenue * 0.3} dkk / Players: {game?.revenue * 0.7} dkk</div>
                    </div>
                    <p className="text-sm text-gray-400 mt-3">{t("game:total_revenue")}</p>
                </div>

                <div
                    className="bg-gray-800 rounded-2xl p-5 border border-gray-700 flex flex-col items-baseline justify-between">
                    <div className="flex items-start justify-between mb-4 w-full">
                        <div className="text-4xl font-bold">
                            {t('game:bets')}: {(game?.bets?.length ?? 0) + (game?.numberOfPhysicalPlayers ?? 0)}
                        </div>

                        <div className="p-3 rounded-xl bg-green-900/30">
                            <Users className="w-6 h-6 text-green-400"/>
                        </div>
                    </div>
                    <div>{t('game:online')}: {game?.bets.length} / {t('game:in_person')}: {game?.numberOfPhysicalPlayers ?? 0}</div>
                    <p className="text-sm mt-3 text-gray-400">{t("total_players")}</p>
                </div>

                <div
                    className="bg-gray-800 rounded-2xl p-5 border border-gray-700 flex flex-col items-baseline justify-between">
                    <div className={"w-full"}>
                        <div className="flex items-start justify-between mb-4">
                            <div className="text-4xl font-bold">
                                {10}
                                <span className={"text-base"}> dkk</span>
                            </div>
                            <div className="p-3 rounded-xl bg-blue-900/30">
                                <Wallet className="w-6 h-6 text-blue-400"/>
                            </div>
                        </div>
                        {/*{game?.drawDate == null && (*/}
                        <div>
                            <button
                                onClick={handleOpenCalculateWinningsModal}
                                className={
                                    "w-full flex flex-row items-center justify-evenly px-3 py-1.5 rounded-lg bg-[#e30613] hover:bg-[#c20510] text-white shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer text-lg"
                                }
                            >
                                <span>{t("game:calculate_winnings")}</span>
                            </button>
                        </div>
                        {/*)}*/}
                    </div>
                    <p className="text-sm mt-3 text-gray-400">{t("game:winnings_per_player")}</p>
                </div>
            </div>

            {game?.bets?.length ?? 0 > 0 ? (
                <div>
                    <div className={"flex flex-row items-baseline pb-4 "}>
                        <p className={"text-3xl mx-0 p-0 text-white"}>{t("game:online_bets")}</p>
                    </div>
                    <div
                        className="relative overflow-visible bg-gray-800 rounded-2xl border border-gray-700 text-white">
                        <table className="w-full text-sm text-left rtl:text-right text-body">
                            <thead>
                            <tr className={"px-2 bg-gray-700"}>
                                <th
                                    scope="col"
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tl-xl"
                                    }
                                >
                                    {t("user")}
                                </th>
                                <th className={"px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider "}>
                                    {t("email")}
                                </th>
                                <th
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                                    }
                                >
                                    {t("selected_numbers")}
                                </th>

                                <th
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                                    }
                                >
                                    {t("price")}
                                </th>
                                <th
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tr-xl"
                                    }
                                >
                                    {t("win")}
                                </th>
                            </tr>
                            </thead>
                            <tbody className={"divide-y divide-gray-700"}>

                            {game?.bets.map((bet, index) => {
                                const isLast = index === game?.bets.length - 1;
                                return (
                                    <tr key={bet.id} className="">
                                        <td
                                            className={`px-3 py-3 ${isLast ? "rounded-bl-xl" : ""}`}>
                                            {bet.user?.name}
                                        </td>
                                        <td className={"px-3 py-3"}>
                                            {bet.user?.email}
                                        </td>
                                        <td className={"px-3 py-3"}>
                                            {bet.selectedNumbers}
                                        </td>
                                        <td
                                            className={`px-3 py-3`}>
                                            {bet.transaction.amount}
                                        </td>
                                        <td
                                            className={`px-3 py-3 ${isLast ? "rounded-br-xl" : ""}`}>
                                            {bet.isWinning ? <CheckCheck color={"#10ff00"}/> : <X color={"#ff0000"}/>}
                                        </td>
                                    </tr>
                                );
                            })}
                            </tbody>
                        </table>
                    </div>
                </div>
            ) : t('no_bets_yet')}

            {isUpdateGameWinningNumbersModalOpen && (
                <div className="fixed inset-0 bg-slate-700/65 flex items-center justify-center z-50">
                    <div
                        className="bg-gray-800 border border-gray-700 rounded-xl shadow-lg w-full max-w-md p-6 text-white">
                        <div className="flex justify-between items-center mb-6">
                            <h2 className="text-2xl font-semibold">
                                {t("game:pick_winning_numbers")}
                            </h2>
                            <button
                                onClick={handleCloseUpdateGameWinningNumbersModal}
                                className="text-gray-500 hover:text-gray-700 cursor-pointer"
                            >
                                <X size={24}/>
                            </button>
                        </div>

                        <div className="gameboard-grid">
                            {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16].map((n) => (
                                <button
                                    key={n}
                                    className={`gameboard-box ${selected.includes(n) ? "selected" : ""}`}
                                    onClick={() => toggle(n)}
                                >
                                    {n}
                                </button>
                            ))}
                        </div>

                        <div>
                            <div className="flex justify-between gap-3 mt-6">
                                <button
                                    type="button"
                                    onClick={handleCloseUpdateGameWinningNumbersModal}
                                    className="px-4 py-2 border border-gray-700 rounded-lg bg-gray-700 hover:bg-gray-600 cursor-pointer"
                                >
                                    {t("cancel")}
                                </button>
                                <button
                                    type="button"
                                    onClick={handleUpdateGameWinningNumbers}
                                    className="px-4 py-2 border border-gray-700 rounded-lg bg-green-500 text-white hover:bg-green-400 cursor-pointer"
                                >
                                    {t("submit")}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {isDrawGameWinnersModalOpen && (
                <div className="fixed inset-0 bg-slate-700/65 flex items-center justify-center z-50">
                    <div
                        className="bg-gray-800 border border-gray-700 rounded-xl shadow-lg w-full max-w-md p-6 text-white">
                        <div className="flex justify-between items-center mb-4">
                            <h2 className="text-2xl font-semibold">
                                {t("game:draw_winners")}
                            </h2>
                            <button
                                onClick={handleCloseDrawGameWinnersModal}
                                className="text-gray-500 hover:text-gray-700 cursor-pointer"
                            >
                                <X size={24}/>
                            </button>
                        </div>
                        <div className={"text-lg text-red-400 py-4"}>
                            {t('game:draw_winners_modal_message')}
                        </div>
                        <div>
                            <div className="flex justify-between gap-3 mt-6">
                                <button
                                    type="button"
                                    onClick={handleCloseDrawGameWinnersModal}
                                    className="px-4 py-2 border border-gray-700 rounded-lg bg-gray-700 hover:bg-gray-600 cursor-pointer"
                                >
                                    {t("cancel")}
                                </button>
                                <button
                                    type="button"
                                    onClick={handleDrawGameWinners}
                                    className="px-4 py-2 border border-gray-700 rounded-lg bg-green-500 text-white hover:bg-green-400 cursor-pointer"
                                >
                                    {t("submit")}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {isCalculateWinningsModalOpen && (
                <div className="fixed inset-0 bg-slate-700/65 flex items-center justify-center z-50">
                    <div
                        className="bg-gray-800 border border-gray-700 rounded-xl shadow-lg w-full max-w-md p-6 text-white">
                        <div className="flex justify-between items-center mb-4">
                            <h2 className="text-2xl font-semibold">
                                {t("game:draw_winners")}
                            </h2>
                            <button
                                onClick={handleCloseCalculateWinningsModal}
                                className="text-gray-500 hover:text-gray-700 cursor-pointer"
                            >
                                <X size={24}/>
                            </button>
                        </div>
                        <div className={"text-lg text-red-400 py-4"}>
                            <input
                                type="number"
                                placeholder={t("game:number_of_physical_players")}
                                value={numberOfPhysicalPlayers}
                                required={true}
                                onChange={(e) => setNumberOfPhysicalPlayers(e.target.value)}
                                className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                            />
                        </div>
                        <div>
                            <div className="flex justify-between gap-3 mt-6">
                                <button
                                    type="button"
                                    onClick={handleCloseCalculateWinningsModal}
                                    className="px-4 py-2 border border-gray-700 rounded-lg bg-gray-700 hover:bg-gray-600 cursor-pointer"
                                >
                                    {t("cancel")}
                                </button>
                                <button
                                    type="button"
                                    onClick={handleCalculateWinnings}
                                    className="px-4 py-2 border border-gray-700 rounded-lg bg-green-500 text-white hover:bg-green-400 cursor-pointer"
                                >
                                    {t("submit")}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

        </div>
    );
}
