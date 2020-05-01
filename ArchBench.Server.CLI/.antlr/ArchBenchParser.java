// Generated from c:\Academics\SoftwareArchitectures\Projects\ArchBench\ArchBench.Server.CLI\ArchBench.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class ArchBenchParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		PORT=1, PATH=2, EXIT=3, STOP=4, INSTALL=5, NEWLINE=6, WHITESPACE=7, Start=8;
	public static final int
		RULE_commands = 0, RULE_command = 1;
	public static final String[] ruleNames = {
		"commands", "command"
	};

	private static final String[] _LITERAL_NAMES = {
		null, null, null, "'exit'", "'stop'", "'install'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, "PORT", "PATH", "EXIT", "STOP", "INSTALL", "NEWLINE", "WHITESPACE", 
		"Start"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "ArchBench.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public ArchBenchParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class CommandsContext extends ParserRuleContext {
		public CommandContext command() {
			return getRuleContext(CommandContext.class,0);
		}
		public TerminalNode NEWLINE() { return getToken(ArchBenchParser.NEWLINE, 0); }
		public CommandsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_commands; }
	}

	public final CommandsContext commands() throws RecognitionException {
		CommandsContext _localctx = new CommandsContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_commands);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(4);
			command();
			setState(5);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CommandContext extends ParserRuleContext {
		public TerminalNode Start() { return getToken(ArchBenchParser.Start, 0); }
		public TerminalNode PORT() { return getToken(ArchBenchParser.PORT, 0); }
		public TerminalNode STOP() { return getToken(ArchBenchParser.STOP, 0); }
		public TerminalNode INSTALL() { return getToken(ArchBenchParser.INSTALL, 0); }
		public TerminalNode PATH() { return getToken(ArchBenchParser.PATH, 0); }
		public CommandContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_command; }
	}

	public final CommandContext command() throws RecognitionException {
		CommandContext _localctx = new CommandContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_command);
		try {
			setState(12);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case Start:
				enterOuterAlt(_localctx, 1);
				{
				setState(7);
				match(Start);
				setState(8);
				match(PORT);
				}
				break;
			case STOP:
				enterOuterAlt(_localctx, 2);
				{
				setState(9);
				match(STOP);
				}
				break;
			case INSTALL:
				enterOuterAlt(_localctx, 3);
				{
				setState(10);
				match(INSTALL);
				setState(11);
				match(PATH);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3\n\21\4\2\t\2\4\3"+
		"\t\3\3\2\3\2\3\2\3\3\3\3\3\3\3\3\3\3\5\3\17\n\3\3\3\2\2\4\2\4\2\2\2\20"+
		"\2\6\3\2\2\2\4\16\3\2\2\2\6\7\5\4\3\2\7\b\7\b\2\2\b\3\3\2\2\2\t\n\7\n"+
		"\2\2\n\17\7\3\2\2\13\17\7\6\2\2\f\r\7\7\2\2\r\17\7\4\2\2\16\t\3\2\2\2"+
		"\16\13\3\2\2\2\16\f\3\2\2\2\17\5\3\2\2\2\3\16";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}